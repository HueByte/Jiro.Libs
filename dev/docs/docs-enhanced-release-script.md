# Enhanced Create Release Script

## 🚀 New Features Added

The `create-release.ps1` script has been significantly enhanced with quality checks and improved git operations workflow.

### ✨ Quality Checks Integration

#### 1. **Code Formatting** (`dotnet format`)

- **Runs**: `dotnet format src/Main.sln --verify-no-changes`
- **Auto-fix**: If formatting issues found, automatically runs `dotnet format src/Main.sln`
- **Skip option**: Use `-SkipFormat` to bypass this step
- **Benefits**: Ensures consistent code formatting before releases

#### 2. **Markdown Linting** (after release notes generation)

- **Runs**: `.\scripts\markdown-lint.ps1 -Fix`
- **Timing**: Executes AFTER release notes file is created
- **Auto-fix**: Automatically fixes markdown formatting issues
- **Skip option**: Use `-SkipLint` to bypass this step
- **Benefits**: Ensures release notes and all markdown follow standards

### 🔄 Improved Git Workflow

#### Enhanced Process Flow:

1. **Quality Checks** (formatting & linting)
2. **Generate Release Notes** → `dev/tags/release_notes_v{version}.md`
3. **Lint Generated File** (markdown quality check)
4. **Commit & Push Changes** (if any formatting/linting changes made)
5. **Create & Push Tag**
6. **GitHub Release Creation**

#### Git Operations:

- **Auto-commit**: Commits formatting and linting changes with message: `chore: format code and lint markdown for release v{version}`
- **Auto-push**: Pushes changes to origin before creating tag
- **Tag creation**: Creates annotated tag with release message
- **Tag push**: Pushes tag to trigger CI/CD workflows

### 📋 New Command Line Options

```powershell
# New parameters added
-SkipFormat     # Skip dotnet format step
-SkipLint       # Skip markdown linting step

# Enhanced help
.\create-release.ps1 -Help
```

### 🎯 Usage Examples

```powershell
# Full release with all quality checks (recommended)
.\create-release.ps1 -Version "v3.0.1"

# Quick release skipping quality checks
.\create-release.ps1 -Version "v3.0.1" -SkipFormat -SkipLint

# Dry run to preview actions
.\create-release.ps1 -Version "v3.0.1" -DryRun

# Auto-version with quality checks
.\create-release.ps1

# With build artifacts
.\create-release.ps1 -AttachBuilds
```

### 🔍 Enhanced Dry Run Output

The dry run now shows a comprehensive overview:

```
🔍 DRY RUN - Actions that would be performed:
1. Quality Checks:
   - Run dotnet format on solution
   - Generate release notes file: dev/tags/release_notes_v3.0.1.md
   - Run markdown lint with auto-fix
2. Git Operations:
   - Commit any formatting/linting changes
   - Push changes to origin/main
   - Create git tag: v3.0.1
   - Push tag to origin
3. Release Creation:
   - Create GitHub release with generated notes
```

### ⚠️ Safety Features

#### Change Detection:

- **Uncommitted changes warning**: Alerts if working directory has changes
- **Branch verification**: Warns if not on main branch
- **Change tracking**: Tracks if formatting/linting made changes
- **Confirmation prompts**: Asks for confirmation before proceeding

#### Error Handling:

- **Build failures**: Stops if dotnet format fails
- **Lint failures**: Continues but reports issues
- **Git failures**: Stops if tag creation or push fails
- **Missing tools**: Warns if markdown-lint script not found

### 🛠️ Technical Implementation

#### Code Formatting Check:

```powershell
$formatResult = dotnet format src/Main.sln --verify-no-changes 2>&1
if ($LASTEXITCODE -ne 0) {
    Write-ColorOutput "⚠️ Code formatting issues detected. Running format..." "Yellow"
    dotnet format src/Main.sln
    $hasChanges = $true
}
```

#### Markdown Linting:

```powershell
$markdownLintScript = Join-Path $PSScriptRoot "markdown-lint.ps1"
if (Test-Path $markdownLintScript) {
    & $markdownLintScript -Fix
    $hasChanges = $true
}
```

#### Git Operations:

```powershell
if ($hasChanges -and -not $DryRun) {
    git add -A
    git commit -m "chore: format code and lint markdown for release $Version"
    git push origin $currentBranch
}
```

### 📊 Benefits

1. **Quality Assurance**: Ensures code and documentation quality before releases
2. **Automation**: Reduces manual steps in release process
3. **Consistency**: Enforces formatting and documentation standards
4. **Reliability**: Comprehensive error handling and safety checks
5. **Transparency**: Clear dry-run preview of all actions

### 🔄 Integration with CI/CD

The enhanced script works seamlessly with existing GitHub Actions:

- **Triggers**: Tag push still triggers `create-release.yml` workflow
- **Quality**: Pre-push quality checks reduce CI failures
- **Consistency**: Automated formatting ensures consistent codebase
- **Documentation**: Auto-linted release notes improve readability

This enhancement transforms the release script from a simple tag creator into a comprehensive release management tool with built-in quality assurance!
