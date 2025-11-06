using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Diagnostics;
using System.Threading.Tasks;
using McMaster.Extensions.CommandLineUtils;

[Command(Name = "dotnet-new-webapi", Description = "Scaffold a new .NET Web API application with user management and business features")]
[HelpOption]
class Program
{
    [Argument(0, Name = "AppName", Description = "The name for your new application")]
    public string AppName { get; }

    [Option(Description = "Output directory (default: current directory)")]
    public string Output { get; } = Directory.GetCurrentDirectory();

    [Option(Description = "Use local template instead of GitHub")]
    public string LocalTemplate { get; }

    [Option(Description = "GitHub repository URL for the template")]
    public string Repository { get; } = "https://github.com/bernard139/Template_API.git";

    [Option(Description = "Branch to use (default: Dev)")]
    public string Branch { get; } = "Dev";

    [Option(Description = "Show verbose output")]
    public bool Verbose { get; }

    private const string TemplateName = "Template";

    public static int Main(string[] args) => CommandLineApplication.Execute<Program>(args);

    private void OnExecute()
    {
        try
        {
            if (string.IsNullOrWhiteSpace(AppName))
            {
                AskForAppNameInteractively();
            }
            else
            {
                ProcessTemplate(AppName).GetAwaiter().GetResult();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Error: {ex.Message}");
            if (Verbose)
            {
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
            }
        }
    }

    private void AskForAppNameInteractively()
    {
        Console.WriteLine("🚀 .NET Web API Template Scaffolder");
        Console.WriteLine("=====================================");
        Console.WriteLine("This tool will create a new .NET Web API application with:");
        Console.WriteLine("✅ User Management (Authentication & Authorization)");
        Console.WriteLine("✅ Business Logic Features");
        Console.WriteLine("✅ Clean Architecture Setup");
        Console.WriteLine("✅ Database Configuration");
        Console.WriteLine("✅ Unit Testing Structure");
        Console.WriteLine();

        Console.Write("Enter your new application name: ");
        var appName = Console.ReadLine()?.Trim();

        if (string.IsNullOrWhiteSpace(appName))
        {
            Console.WriteLine("App name cannot be empty. Please try again.");
            return;
        }

        ProcessTemplate(appName).GetAwaiter().GetResult();
    }

    private async Task ProcessTemplate(string appName)
    {
        var sanitizedName = SanitizeAppName(appName);
        var targetDir = Path.Combine(Output, sanitizedName);

        if (Directory.Exists(targetDir))
        {
            Console.WriteLine($"❌ Directory '{sanitizedName}' already exists!");
            Console.WriteLine("Please choose a different application name or delete the existing directory.");
            return;
        }

        // Use local template if specified, otherwise use GitHub
        if (!string.IsNullOrWhiteSpace(LocalTemplate) && Directory.Exists(LocalTemplate))
        {
            Console.WriteLine($"\n📁 Creating new application: {sanitizedName}");
            Console.WriteLine($"📂 Using local template from: {LocalTemplate}");
            Console.WriteLine("⏳ This may take a moment...");
            CopyAndTransformTemplate(LocalTemplate, targetDir, sanitizedName);
            Console.WriteLine($"\n✅ Successfully created application: {sanitizedName}");
        }
        else
        {
            Console.WriteLine($"\n📁 Creating new application: {sanitizedName}");
            Console.WriteLine($"🌐 Downloading template from GitHub...");
            Console.WriteLine($"📥 Repository: {Repository}");
            Console.WriteLine($"🎯 Branch: {Branch}");
            Console.WriteLine("⏳ This may take a moment...");

            if (await CloneAndScaffoldTemplate(sanitizedName, targetDir))
            {
                Console.WriteLine($"\n✅ Successfully created application: {sanitizedName}");
            }
            else
            {
                Console.WriteLine($"\n❌ Failed to create application.");
                Console.WriteLine("Please check your internet connection and ensure Git is installed.");
                Console.WriteLine($"💡 You can also use a local template: dotnet-new-webapi {appName} --local-template \"C:\\path\\to\\template\"");
                return;
            }
        }

        PrintNextSteps(sanitizedName);
    }

    private async Task<bool> CloneAndScaffoldTemplate(string appName, string targetDir)
    {
        var tempDir = Path.Combine(Path.GetTempPath(), $"template_{Guid.NewGuid()}");

        try
        {
            Directory.CreateDirectory(tempDir);

            // Clone the repository
            if (!await CloneRepository(Repository, Branch, tempDir))
            {
                return false;
            }

            // Copy and transform the template
            CopyAndTransformTemplate(tempDir, targetDir, appName);
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Error during template creation: {ex.Message}");
            return false;
        }
        finally
        {
            // Clean up temporary directory
            try
            {
                if (Directory.Exists(tempDir))
                {
                    Directory.Delete(tempDir, true);
                }
            }
            catch (Exception ex)
            {
                LogVerbose($"Warning: Could not clean up temporary directory: {ex.Message}");
            }
        }
    }

    private async Task<bool> CloneRepository(string repoUrl, string branch, string targetDir)
    {
        try
        {
            var processInfo = new ProcessStartInfo
            {
                FileName = "git",
                Arguments = $"clone --branch {branch} --depth 1 {repoUrl} \"{targetDir}\"",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true
            };

            using (var process = new Process { StartInfo = processInfo })
            {
                process.Start();

                var outputTask = process.StandardOutput.ReadToEndAsync();
                var errorTask = process.StandardError.ReadToEndAsync();

                if (!process.WaitForExit(120000))
                {
                    process.Kill();
                    Console.WriteLine("❌ Git clone timed out after 2 minutes.");
                    return false;
                }

                var output = await outputTask;
                var error = await errorTask;

                if (process.ExitCode == 0)
                {
                    return true;
                }
                else
                {
                    Console.WriteLine($"❌ Git clone failed: {error}");

                    if (error.Contains("not found"))
                    {
                        Console.WriteLine($"💡 Please check that the repository URL is correct: {repoUrl}");
                        Console.WriteLine($"💡 And that the branch exists: {branch}");
                    }
                    else if (error.Contains("command not found") || error.Contains("is not recognized"))
                    {
                        Console.WriteLine("💡 Git is not installed or not in PATH. Please install Git from: https://git-scm.com/");
                    }

                    return false;
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Error during Git clone: {ex.Message}");
            if (ex is System.ComponentModel.Win32Exception)
            {
                Console.WriteLine("💡 Git is not installed or not in PATH. Please install Git from: https://git-scm.com/");
            }
            return false;
        }
    }

    private void CopyAndTransformTemplate(string sourceDir, string targetDir, string appName)
    {
        LogVerbose($"Source: {sourceDir}");
        LogVerbose($"Target: {targetDir}");

        // Simple exclusion patterns
        var excludePatterns = new[]
        {
            ".git", "bin", "obj", "Template.CLI", "CLI", "nupkg", ".vs", "Debug", "Release"
        };

        var excludeFiles = new[]
        {
            "build.ps1", "pack.ps1", "install-tool.ps1", "build.bat", "pack.bat",
            "install-tool.bat", "test-tool.ps1", "test-tool.bat", "check-structure.ps1",
            ".gitignore", ".gitattributes", "README.md", "USAGE.md", "SETUP.md"
        };

        try
        {
            // Use a recursive approach to ensure all directories are copied
            CopyDirectoryRecursive(sourceDir, targetDir, appName, excludePatterns, excludeFiles);

            LogVerbose("Template copying completed");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Error copying template: {ex.Message}");
            throw;
        }
    }

    private void CopyDirectoryRecursive(string sourceDir, string targetDir, string appName, string[] excludePatterns, string[] excludeFiles)
    {
        // Create target directory if it doesn't exist
        Directory.CreateDirectory(targetDir);

        // Get all files and directories in the current source directory
        foreach (var file in Directory.GetFiles(sourceDir))
        {
            var fileName = Path.GetFileName(file);

            // Skip excluded files
            if (excludeFiles.Any(exclude => fileName.Equals(exclude, StringComparison.OrdinalIgnoreCase)))
            {
                LogVerbose($"Skipping excluded file: {file}");
                continue;
            }

            var relativePath = GetRelativePath(sourceDir, file);
            var newRelativePath = TransformPath(relativePath, appName);
            var targetFile = Path.Combine(targetDir, newRelativePath);

            // Ensure target directory exists
            var targetFileDir = Path.GetDirectoryName(targetFile);
            if (!Directory.Exists(targetFileDir))
            {
                Directory.CreateDirectory(targetFileDir);
            }

            ProcessFile(file, targetFile, appName);
        }

        // Process subdirectories
        foreach (var directory in Directory.GetDirectories(sourceDir))
        {
            var dirName = Path.GetFileName(directory);

            // Skip excluded directories
            if (excludePatterns.Any(pattern =>
                dirName.IndexOf(pattern, StringComparison.OrdinalIgnoreCase) >= 0))
            {
                LogVerbose($"Skipping excluded directory: {directory}");
                continue;
            }

            var relativePath = GetRelativePath(sourceDir, directory);
            var newRelativePath = TransformPath(relativePath, appName);
            var newTargetDir = Path.Combine(targetDir, newRelativePath);

            CopyDirectoryRecursive(directory, newTargetDir, appName, excludePatterns, excludeFiles);
        }
    }

    private string TransformPath(string path, string appName)
    {
        // Replace Template with appName in the path
        return path.Replace(TemplateName, appName);
    }

    private void ProcessFile(string sourceFile, string targetFile, string appName)
    {
        var extension = Path.GetExtension(sourceFile).ToLower();
        var textFileExtensions = new[] { ".cs", ".csproj", ".sln", ".config", ".json", ".xml", ".txt", ".md", ".props", ".targets", ".resx", ".config", ".editorconfig" };

        if (textFileExtensions.Contains(extension))
        {
            try
            {
                var content = File.ReadAllText(sourceFile);
                var newContent = TransformContent(content, appName);
                File.WriteAllText(targetFile, newContent);
                LogVerbose($"Processed: {targetFile}");
            }
            catch (Exception ex)
            {
                LogVerbose($"Error processing text file {sourceFile}: {ex.Message}");
                // If text processing fails, copy as binary
                File.Copy(sourceFile, targetFile, true);
                LogVerbose($"Copied as binary: {targetFile}");
            }
        }
        else
        {
            // Copy binary files as-is
            File.Copy(sourceFile, targetFile, true);
            LogVerbose($"Copied binary: {targetFile}");
        }
    }

    private string TransformContent(string content, string appName)
    {
        // Comprehensive replacements for all possible occurrences
        var transformed = content
            .Replace(TemplateName, appName)
            .Replace($"namespace {TemplateName}.", $"namespace {appName}.")
            .Replace($"namespace {TemplateName}", $"namespace {appName}")
            .Replace($"using {TemplateName}.", $"using {appName}.")
            .Replace($"using {TemplateName}", $"using {appName}")
            .Replace($"{TemplateName}.API", $"{appName}.API")
            .Replace($"{TemplateName}.Application", $"{appName}.Application")
            .Replace($"{TemplateName}.Domain", $"{appName}.Domain")
            .Replace($"{TemplateName}.Infrastructure", $"{appName}.Infrastructure")
            .Replace($"{TemplateName}.Persistence", $"{appName}.Persistence")
            .Replace($"{TemplateName}.Identity", $"{appName}.Identity")
            .Replace($"{TemplateName}.Common", $"{appName}.Common")
            .Replace($"{TemplateName}.Misc", $"{appName}.Misc")
            .Replace($"<RootNamespace>{TemplateName}", $"<RootNamespace>{appName}")
            .Replace($"<AssemblyName>{TemplateName}", $"<AssemblyName>{appName}");

        return transformed;
    }

    private void CleanBuildArtifacts(string targetDir)
    {
        try
        {
            // Remove all bin and obj directories to avoid build conflicts
            var binDirs = Directory.GetDirectories(targetDir, "bin", SearchOption.AllDirectories);
            var objDirs = Directory.GetDirectories(targetDir, "obj", SearchOption.AllDirectories);

            foreach (var dir in binDirs.Concat(objDirs))
            {
                try
                {
                    Directory.Delete(dir, true);
                    LogVerbose($"Cleaned build artifacts: {dir}");
                }
                catch (Exception ex)
                {
                    LogVerbose($"Warning: Could not clean {dir}: {ex.Message}");
                }
            }
        }
        catch (Exception ex)
        {
            LogVerbose($"Warning: Error cleaning build artifacts: {ex.Message}");
        }
    }

    private void CreateGitIgnore(string targetDir)
    {
        var gitignoreContent = @"## Ignore Visual Studio temporary files, build results, and
## files generated by popular Visual Studio add-ons.

# User-specific files
*.rsuser
*.suo
*.user
*.userosscache
*.sln.docstates

# Build results
[Dd]ebug/
[Dd]ebugPublic/
[Rr]elease/
[Rr]eleases/
x64/
x86/
bld/
[Bb]in/
[Oo]bj/

# Visual Studio files
.vs/

# Other
*.nupkg
";
        File.WriteAllText(Path.Combine(targetDir, ".gitignore"), gitignoreContent.Trim());
    }

    private string GetRelativePath(string fromPath, string toPath)
    {
        var fromUri = new Uri(fromPath + Path.DirectorySeparatorChar);
        var toUri = new Uri(toPath);
        var relativeUri = fromUri.MakeRelativeUri(toUri);
        return Uri.UnescapeDataString(relativeUri.ToString().Replace('/', Path.DirectorySeparatorChar));
    }

    private string SanitizeAppName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return "MyApp";

        var sanitized = Regex.Replace(name, @"[^a-zA-Z0-9_.-]", "");

        if (sanitized.Length > 0 && !char.IsLetter(sanitized[0]))
        {
            sanitized = "App" + sanitized;
        }

        return string.IsNullOrWhiteSpace(sanitized) ? "MyApp" : sanitized;
    }

    private void PrintNextSteps(string appName)
    {
        Console.WriteLine("\n🎉 Your application is ready! Next steps:");
        Console.WriteLine("===========================================");
        Console.WriteLine($"1. 📁 Navigate to project: cd {appName}");
        Console.WriteLine($"2. 🔧 Restore packages: dotnet restore");
        Console.WriteLine($"3. 🏗️  Build solution: dotnet build");
        Console.WriteLine($"4. 🚀 Run the API: dotnet run --project {appName}.API");
    }

    private void LogVerbose(string message)
    {
        if (Verbose)
        {
            Console.WriteLine($"[VERBOSE] {message}");
        }
    }
}