using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using McMaster.Extensions.CommandLineUtils;

[Command(Name = "dotnet-new-webapi", Description = "Scaffold a new .NET Web API application with user management and business features")]
[HelpOption]
class Program
{
    [Argument(0, Name = "AppName", Description = "The name for your new application")]
    public string AppName { get; }

    [Option(Description = "Output directory (default: current directory)")]
    public string Output { get; } = Directory.GetCurrentDirectory();

    [Option(Description = "Template directory path")]
    public string TemplatePath { get; }

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
                ProcessTemplate(AppName);
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

        ProcessTemplate(appName);
    }

    private void ProcessTemplate(string appName)
    {
        var sanitizedName = SanitizeAppName(appName);
        var targetDir = Path.Combine(Output, sanitizedName);

        if (Directory.Exists(targetDir))
        {
            Console.WriteLine($"❌ Directory '{sanitizedName}' already exists!");
            Console.WriteLine("Please choose a different application name or delete the existing directory.");
            return;
        }

        // Find the template directory
        var templateDirectory = FindTemplateDirectory();
        if (templateDirectory == null)
        {
            Console.WriteLine("❌ Could not find template files.");
            Console.WriteLine($"Please run this tool from your template directory or use --template-path option.");
            Console.WriteLine($"Example: dotnet-new-webapi {appName} --template-path \"C:\\Users\\HP\\source\\repos\\Template_API\"");
            return;
        }

        Console.WriteLine($"\n📁 Creating new application: {sanitizedName}");
        Console.WriteLine($"📂 Using template from: {templateDirectory}");
        Console.WriteLine("⏳ This may take a moment...");

        // Copy and transform the template
        CopyAndTransformTemplate(templateDirectory, targetDir, sanitizedName);

        Console.WriteLine($"\n✅ Successfully created application: {sanitizedName}");
        PrintNextSteps(sanitizedName);
    }

    private string FindTemplateDirectory()
    {
        // If template path is provided, use it
        if (!string.IsNullOrWhiteSpace(TemplatePath) && Directory.Exists(TemplatePath))
        {
            return TemplatePath;
        }

        // Check if current directory has template projects
        var currentDir = Directory.GetCurrentDirectory();
        if (HasTemplateProjects(currentDir))
            return currentDir;

        // Check common template locations
        var possiblePaths = new[]
        {
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "source", "repos", "Template_API"),
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "source", "repos", "Template"),
            @"C:\Users\HP\source\repos\Template_API" // Your specific path
        };

        foreach (var path in possiblePaths)
        {
            if (Directory.Exists(path) && HasTemplateProjects(path))
            {
                return path;
            }
        }

        return null;
    }

    private bool HasTemplateProjects(string directory)
    {
        try
        {
            // Check for your specific project structure
            var hasApi = Directory.Exists(Path.Combine(directory, "Template.API")) ||
                         File.Exists(Path.Combine(directory, "Template.API", "Template.API.csproj"));

            var hasApplication = Directory.Exists(Path.Combine(directory, "Template.Application")) ||
                                File.Exists(Path.Combine(directory, "Template.Application", "Template.Application.csproj"));

            var hasPersistence = Directory.Exists(Path.Combine(directory, "Template.Persistence")) ||
                                File.Exists(Path.Combine(directory, "Template.Persistence", "Template.Persistence.csproj"));

            LogVerbose($"Checking template in {directory}: API={hasApi}, Application={hasApplication}, Persistence={hasPersistence}");

            return hasApi && hasApplication; // At least need API and Application
        }
        catch (Exception ex)
        {
            LogVerbose($"Error checking template: {ex.Message}");
            return false;
        }
    }

    private void CopyAndTransformTemplate(string sourceDir, string targetDir, string appName)
    {
        LogVerbose($"Source: {sourceDir}");
        LogVerbose($"Target: {targetDir}");

        // Get all files and directories from the template
        var allItems = Directory.GetFileSystemEntries(sourceDir, "*", SearchOption.AllDirectories)
            .Where(item =>
                !item.Contains("\\.git\\") &&
                !item.Contains("\\bin\\") &&
                !item.Contains("\\obj\\") &&
                !item.Contains("\\Template.CLI\\") && // Exclude CLI tool itself
                !item.Contains("\\nupkg\\") &&
                !item.Contains("\\.vs\\") &&
                !item.EndsWith(".nupkg") &&
                !item.Contains("\\Debug\\") &&
                !item.Contains("\\Release\\")
            )
            .ToList();

        LogVerbose($"Found {allItems.Count} items to process");

        // First pass: Create directory structure
        foreach (var item in allItems)
        {
            if (Directory.Exists(item))
            {
                var relativePath = GetRelativePath(sourceDir, item);
                var newRelativePath = relativePath.Replace(TemplateName, appName);
                var newDir = Path.Combine(targetDir, newRelativePath);

                Directory.CreateDirectory(newDir);
                LogVerbose($"Created directory: {newDir}");
            }
        }

        // Second pass: Copy and transform files
        foreach (var item in allItems.Where(File.Exists))
        {
            var relativePath = GetRelativePath(sourceDir, item);
            var newRelativePath = relativePath.Replace(TemplateName, appName);
            var targetFile = Path.Combine(targetDir, newRelativePath);

            var extension = Path.GetExtension(item).ToLower();
            var textFileExtensions = new[] { ".cs", ".csproj", ".sln", ".config", ".json", ".xml", ".txt", ".md", ".yml", ".yaml", ".props", ".targets", ".config" };

            if (textFileExtensions.Contains(extension))
            {
                try
                {
                    var content = File.ReadAllText(item);
                    var newContent = content.Replace(TemplateName, appName);
                    // Also replace namespace references
                    newContent = newContent.Replace($"namespace {TemplateName}", $"namespace {appName}");
                    newContent = newContent.Replace($"using {TemplateName}", $"using {appName}");

                    File.WriteAllText(targetFile, newContent);
                    LogVerbose($"Processed: {targetFile}");
                }
                catch (Exception ex)
                {
                    LogVerbose($"Error processing {item}: {ex.Message}");
                    // If we can't read as text, copy as binary
                    File.Copy(item, targetFile, true);
                }
            }
            else
            {
                // Copy binary files as-is
                File.Copy(item, targetFile, true);
                LogVerbose($"Copied: {targetFile}");
            }
        }

        // Update solution file name and content
        var solutionFiles = Directory.GetFiles(targetDir, "*.sln");
        foreach (var solutionFile in solutionFiles)
        {
            var newSolutionName = Path.GetFileName(solutionFile).Replace(TemplateName, appName);
            var newSolutionPath = Path.Combine(Path.GetDirectoryName(solutionFile), newSolutionName);
            File.Move(solutionFile, newSolutionPath);
            LogVerbose($"Renamed solution: {newSolutionPath}");
        }

        LogVerbose("Template copying completed");
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
        Console.WriteLine($"\n📚 Your application includes all your template features:");
        Console.WriteLine($"   • User Management & Authentication");
        Console.WriteLine($"   • Business Logic");
        Console.WriteLine($"   • Database Layer");
        Console.WriteLine($"   • API Controllers");
        Console.WriteLine($"   • And much more!");
    }

    private void LogVerbose(string message)
    {
        if (Verbose)
        {
            Console.WriteLine($"[VERBOSE] {message}");
        }
    }
}