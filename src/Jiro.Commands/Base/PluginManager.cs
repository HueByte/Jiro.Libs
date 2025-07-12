using System.Collections.Concurrent;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Jiro.Commands.Base
{
    /// <summary>
    /// Manages the loading, building, and registration of plugins and modules.
    /// </summary>
    public class PluginManager
    {
        private readonly ILogger? _logger;
        private readonly IServiceCollection _services;
        private readonly ConfigurationManager _configurationManager;
        private readonly ConcurrentBag<string> _modulePaths = new();
        private readonly List<Assembly> _assemblies = new();
        private readonly List<string> _moduleNames = new();
        private List<Type>? _serviceConfigurators;

        /// <summary>
        /// Initializes a new instance of the <see cref="PluginManager"/> class.
        /// </summary>
        /// <param name="services">The service collection to use for registration.</param>
        /// <param name="configurationManager">The configuration manager for app settings.</param>
        /// <param name="logger">The logger for diagnostic output.</param>
        public PluginManager(IServiceCollection services, ConfigurationManager configurationManager, ILogger? logger = null)
        {
            AppDomain.CurrentDomain.ReflectionOnlyAssemblyResolve += ResolveAssembly!;
            AppDomain.CurrentDomain.AssemblyResolve += ResolveAssembly!;

            _services = services;
            _configurationManager = configurationManager;
            _logger = logger;
        }

        /// <summary>
        /// Builds modules included in appsettings for debug builds.
        /// </summary>
        /// <param name="modulePaths">The paths to the modules to build.</param>
        public void BuildDevModules(string[]? modulePaths)
        {
            if (modulePaths is null) return;

            var modules = modulePaths.Select(e => Path.GetFullPath(e)).ToArray();
            var debugPath = @"bin\Debug";

            if (modules is null) return;

            _logger?.LogInformation("Building {length} module(s)", modules.Length);

            Task[] buildTasks = new Task[modules.Length];
            for (int i = 0; i < buildTasks.Length; i++)
            {
                int localScope = i;
                buildTasks[i] = Task.Run(async () =>
                {
                    _logger?.LogInformation("Building: [{module}]", modules[localScope]);
                    var path = modules[localScope];

                    try
                    {
                        await RunBuildCommandAsync(path);

                        var completepath = Path.Combine(path, debugPath);
                        var outputFolder = Directory.GetDirectories(completepath, "net*").First();

                        _logger?.LogInformation("[{moduleName}]::Build done.\nOutput folder: {outputFolder}", modules[localScope], outputFolder);
                        _modulePaths.Add(outputFolder);
                    }
                    catch (Exception)
                    {
                        _logger?.LogError("Couldn't build module [{module}]", modules[localScope]);
                    }
                });
            }

            Task.WaitAll(buildTasks);
        }

        /// <summary>
        /// Navigates to path and runs `dotnet build` command
        /// </summary>
        /// <param name="path">Path to navigate to</param>
        /// <returns></returns>
        private async Task RunBuildCommandAsync(string path)
        {
            if (!Directory.Exists(path))
                throw new ArgumentException($"Couldn't find {path}");

            using Process cmd = new();
            cmd.StartInfo.FileName = GetTerminal(GetOS());
            cmd.StartInfo.RedirectStandardInput = true;
            cmd.StartInfo.RedirectStandardOutput = true;
            cmd.StartInfo.RedirectStandardError = true;
            cmd.StartInfo.CreateNoWindow = true;
            cmd.StartInfo.UseShellExecute = false;

            string navigateCommand = $"cd \"{path}\"";
            string buildCommand = "dotnet build";
            _logger?.LogInformation("Running commands: \n[{c1}]\n[{c2}]", navigateCommand, buildCommand);

            cmd.Start();

            var stdOutTask = cmd.StandardOutput.ReadToEndAsync();
            var errTask = cmd.StandardError.ReadToEndAsync();

            await cmd.StandardInput.WriteLineAsync(navigateCommand);
            await cmd.StandardInput.WriteLineAsync(buildCommand);
            await cmd.StandardInput.FlushAsync();
            cmd.StandardInput.Close();

            await cmd.WaitForExitAsync();
            await Task.WhenAll(stdOutTask, errTask);

            var outResult = await stdOutTask;
            _ = await errTask;

            _logger?.LogInformation("Build output:\n {output}", outResult);
        }

        private static string GetOS()
        {
            return RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
                ? "windows"
                : RuntimeInformation.IsOSPlatform(OSPlatform.Linux)
                    ? "linux"
                    : "osx";
        }

        private static string GetTerminal(string platform) => platform switch
        {
            "windows" => "cmd.exe",
            "linux" => "bash",
            "osx" => "zsh",
            _ => throw new Exception($"Couldn't find terminal to use for {platform}")
        };

        public void LoadModuleAssemblies()
        {
            var modulePath = AppContext.BaseDirectory + "Modules";
            if (!Directory.Exists(modulePath))
                Directory.CreateDirectory(modulePath);

            List<string> paths = new() { modulePath };

            paths.AddRange(_modulePaths);

            List<string> dllFiles = new();
            foreach (var path in paths)
                dllFiles.AddRange(GetDllFiles(path).ToList());

            var folders = Directory.GetDirectories(modulePath);

            dllFiles.AddRange(folders.Select(folder => GetDllFiles(folder)).SelectMany(e => e));

            foreach (var dll in dllFiles)
            {
                _logger?.LogInformation("Loading {dll}", dll);

                _assemblies.Add(Assembly.LoadFile(dll));
            }

            _serviceConfigurators = _assemblies
                .SelectMany(e => e.GetTypes())
                .Where(type => !type.IsInterface && typeof(IPlugin).IsAssignableFrom(type))
                .ToList();

        }
        /// <summary>
        /// Loads controllers from loaded modular assemblies
        /// </summary>
        public void LoadModuleControllers()
        {
            try
            {
                _services
                    .AddControllers()
                    .ConfigureApplicationPartManager((manager) =>
                    {
                        foreach (var asm in _assemblies)
                            manager.ApplicationParts.Add(new AssemblyPart(asm));

                    })
                    .AddControllersAsServices();
            }
            catch (ReflectionTypeLoadException ex)
            {
                StringBuilder sb = new();
                foreach (var exSub in ex.LoaderExceptions)
                {
                    sb.AppendLine(exSub!.Message);
                    if (exSub is FileNotFoundException exFileNotFound)
                    {
                        if (!string.IsNullOrEmpty(exFileNotFound.FusionLog))
                        {
                            sb.AppendLine("Fusion Log:");
                            sb.AppendLine(exFileNotFound.FusionLog);
                        }
                    }
                    sb.AppendLine();
                }

                string errorMessage = sb.ToString();
                _logger?.LogError(errorMessage);
            }
        }

        /// <summary>
        /// Runs `IServiceConfigurator` of loaded modular assemblies
        /// </summary>
        public void RegisterModuleServices()
        {
            try
            {
                if (_serviceConfigurators is null) return;

                foreach (var serviceConfigurator in _serviceConfigurators)
                {
                    if (Activator.CreateInstance(serviceConfigurator) is not IPlugin configurator) continue;

                    _logger?.LogInformation("Running {pluginName} config registrator", configurator.PluginName);
                    configurator.RegisterAppConfigs(_configurationManager);

                    _logger?.LogInformation("Running {pluginName} service registrator", configurator.PluginName);
                    configurator.RegisterServices(_services);

                    _moduleNames.Add(configurator.PluginName);
                }
            }
            catch (ReflectionTypeLoadException ex)
            {
                StringBuilder sb = new();
                foreach (var exSub in ex.LoaderExceptions)
                {
                    sb.AppendLine(exSub!.Message);
                    if (exSub is FileNotFoundException exFileNotFound)
                    {
                        if (!string.IsNullOrEmpty(exFileNotFound.FusionLog))
                        {
                            sb.AppendLine("Fusion Log:");
                            sb.AppendLine(exFileNotFound.FusionLog);
                        }
                    }
                    sb.AppendLine();
                }

                string errorMessage = sb.ToString();
                _logger?.LogError(errorMessage);
            }
        }

        public void RegisterAppExtensions(IApplicationBuilder app)
        {
            if (_serviceConfigurators is null) return;

            foreach (var serviceConfigurator in _serviceConfigurators)
            {
                if (Activator.CreateInstance(serviceConfigurator) is not IPlugin configurator) continue;

                _logger?.LogInformation("Running {pluginName} app extension registrator", configurator.PluginName);
                configurator.RegisterAppExtensions(app);

                _moduleNames.Add(configurator.PluginName);
            }
        }

        /// <summary>
        /// Validates modules based on `IServiceConfigurator.ConfiguratorName` and appsettings RequiredModules 
        /// </summary>
        /// <exception cref="Exception"></exception>
        public void ValidateModules(string[]? requiredModules)
        {
            var lackingModules = requiredModules
                ?.Where(m => !_moduleNames.Contains(m));

            if (lackingModules is not null && lackingModules.Any())
                throw new Exception($"Couldn't find {string.Join(',', lackingModules)} module");
        }

        /// <summary>
        /// Assembly resolver
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <returns></returns>
        private Assembly? ResolveAssembly(Object sender, ResolveEventArgs e)
        {
            Assembly? res = _assemblies.FirstOrDefault(asm => asm.FullName == e.Name);

            if (res is null)
            {
                var args = e.Name.Split(',');
                res = _assemblies.FirstOrDefault(asm => asm.FullName!.Contains(args[0]));
            }

            return res;
        }

        /// <summary>
        /// Gets DLL files based on `*.dll` pattern
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private static string[] GetDllFiles(string path) => Directory.GetFiles(path, "*.dll");
    }
}
