# EditorConfig Usage in Jiro.Libs

This project uses [EditorConfig](https://editorconfig.org/) to maintain consistent coding styles across different editors and IDEs.

## What is EditorConfig?

EditorConfig helps maintain consistent coding styles for multiple developers working on the same project across various editors and IDEs. The `.editorconfig` file at the root of this repository defines coding styles for different file types.

## Supported File Types

The `.editorconfig` configuration covers:

- **C# files (*.cs)**: Tab indentation (4 spaces), comprehensive C# style rules
- **JSON/XML files (*.json, *.xml, *.csproj, etc.)**: Space indentation (2 spaces)
- **YAML files (*.yml, *.yaml)**: Space indentation (2 spaces)
- **Markdown files (*.md)**: Space indentation (2 spaces), trailing whitespace preserved
- **PowerShell scripts (*.ps1)**: Space indentation (4 spaces), UTF-8 with BOM
- **Shell scripts (*.sh, *.bash)**: Space indentation (2 spaces), LF line endings
- **Web files (*.html, *.css, *.js, *.ts)**: Space indentation (2 spaces)

## Editor Support

### Visual Studio Code

- Install the [EditorConfig extension](https://marketplace.visualstudio.com/items?itemName=EditorConfig.EditorConfig)
- The `.vscode/settings.json` file includes additional settings that complement EditorConfig
- Recommended extensions are listed in `.vscode/extensions.json`

### Visual Studio

- Built-in EditorConfig support (Visual Studio 2017 and later)
- The IDE will automatically apply the settings defined in `.editorconfig`

### JetBrains IDEs (Rider, IntelliJ, etc.)

- Built-in EditorConfig support
- Enable "Enable EditorConfig support" in Settings → Editor → Code Style

## CI/CD Integration

The project's continuous integration workflows respect EditorConfig settings:

- **Code Formatting**: The `dotnet format` command in CI checks that C# code follows EditorConfig rules
- **Markdown Linting**: Uses `.markdownlint.json` config that aligns with EditorConfig markdown settings
- **Line Endings**: `.gitattributes` ensures consistent line endings that match EditorConfig settings

## Development Workflow

1. **Install recommended extensions** for your editor (see `.vscode/extensions.json`)
2. **EditorConfig will automatically**:
   - Set correct indentation (tabs vs spaces)
   - Set correct indent size
   - Set correct line endings (LF vs CRLF)
   - Trim trailing whitespace
   - Insert final newline
3. **Before committing**:
   - Run `dotnet format` to ensure C# code follows the style guide
   - The CI pipeline will verify formatting compliance

## File Structure

```text
.editorconfig              # Main EditorConfig file (applies to entire repo)
.gitattributes            # Git attributes for line endings
.vscode/
  ├── settings.json       # VS Code specific settings
  └── extensions.json     # Recommended extensions
dev/config/
  └── .markdownlint.json  # Markdown linting rules
```

## Customization

If you need to modify coding standards:

1. Update `.editorconfig` for general formatting rules
2. Update language-specific config files (e.g., `.markdownlint.json`)
3. Update `.vscode/settings.json` for VS Code specific enhancements
4. Ensure CI workflows respect the new settings

## Troubleshooting

### EditorConfig not working?

- Ensure your editor has EditorConfig support enabled
- Check that the `.editorconfig` file is at the repository root
- Restart your editor after installing EditorConfig extension

### Formatting conflicts?

- EditorConfig takes precedence over editor defaults
- Check for conflicting editor-specific configuration files
- Ensure `.vscode/settings.json` complements rather than conflicts with EditorConfig

### CI formatting failures?

- Run `dotnet format` locally before committing
- Check that your local EditorConfig settings match the repository
- Verify that your editor is applying EditorConfig rules correctly
