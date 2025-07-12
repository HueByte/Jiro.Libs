namespace Jiro.Commands.Attributes;

/// <summary>
/// Applied to a class, marks it ready to be used as a command module
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
/// <summary>
/// Attribute applied to a class to mark it as a command module.
/// </summary>
public class CommandModuleAttribute : Attribute
{
    /// <summary>
    /// Gets the name of the command module.
    /// </summary>
    public string ModuleName { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="CommandModuleAttribute"/> class.
    /// </summary>
    /// <param name="moduleName">The name of the command module.</param>
    public CommandModuleAttribute(string moduleName)
    {
        ModuleName = moduleName;
    }
}