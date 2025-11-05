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

    [Option(Description = "Check for template updates")]
    public bool CheckUpdates { get; }

    [Option(Description = "Force update from GitHub")]
    public bool Update { get; }

    private const string TemplateName = "Template";

    public static int Main(string[] args) => CommandLineApplication.Execute<Program>(args);

    private void OnExecute()
    {
        CheckForUpdates();

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

    private void CheckForUpdates()
    {
        if (CheckUpdates || Update)
        {
            Console.WriteLine("🔍 Checking for template updates...");
            Console.WriteLine("To update your template:");
            Console.WriteLine("1. Pull the latest changes from your Git repository");
            Console.WriteLine("2. Run: dotnet tool update -g Template.CLI");
            Console.WriteLine("3. Or rebuild: .\\build.ps1 && .\\pack.ps1 && dotnet tool update -g Template.CLI");

            if (Update)
            {
                Environment.Exit(0);
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

        // Define patterns to exclude - both directories and files
        var excludePatterns = new[]
        {
            "\\.git",                  // Git directory and files
            "\\bin\\",                 // Build output
            "\\obj\\",                 // Build objects
            "\\Template\\.CLI",        // CLI tool itself (with dot)
            "\\CLI\\",                 // CLI directory
            "\\nupkg\\",               // NuGet packages
            "\\.vs\\",                 // Visual Studio files
            "\\Debug\\",               // Debug builds
            "\\Release\\",             // Release builds
            "\\TestScaffold",          // Test directories
            "test-tool",               // Test tool scripts
            "check-structure"          // Diagnostic scripts
        };

        // Specific files to exclude (case insensitive)
        var excludeFiles = new[]
        {
            "build.ps1",
            "pack.ps1",
            "install-tool.ps1",
            "build.bat",
            "pack.bat",
            "install-tool.bat",
            "test-tool.ps1",
            "test-tool.bat",
            "check-structure.ps1",
            ".gitignore",
            ".gitattributes",
            "README.md",               // Exclude template's README
            "USAGE.md",                // Exclude usage instructions
            "SETUP.md"                 // Exclude setup instructions
        };

        // Get all files and directories from the template, excluding unwanted items
        var allItems = Directory.GetFileSystemEntries(sourceDir, "*", SearchOption.AllDirectories)
            .Where(item => !ShouldExcludeItem(item, sourceDir, excludePatterns, excludeFiles))
            .ToList();

        LogVerbose($"Found {allItems.Count} items to process after filtering");

        // First pass: Create directory structure
        foreach (var item in allItems)
        {
            if (Directory.Exists(item))
            {
                var relativePath = GetRelativePath(sourceDir, item);

                // Skip if this directory should be excluded
                if (ShouldExcludeItem(item, sourceDir, excludePatterns, excludeFiles))
                    continue;

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

            // Skip if this file should be excluded
            if (ShouldExcludeItem(item, sourceDir, excludePatterns, excludeFiles))
                continue;

            var newRelativePath = relativePath.Replace(TemplateName, appName);
            var targetFile = Path.Combine(targetDir, newRelativePath);

            var extension = Path.GetExtension(item).ToLower();
            var textFileExtensions = new[] {
                ".cs", ".csproj", ".sln", ".config", ".json", ".xml",
                ".txt", ".md", ".yml", ".yaml", ".props", ".targets",
                ".config", ".editorconfig", ".gitattributes"
            };

            if (textFileExtensions.Contains(extension))
            {
                try
                {
                    var content = File.ReadAllText(item);
                    var newContent = content.Replace(TemplateName, appName);
                    // Also replace namespace references more comprehensively
                    newContent = newContent.Replace($"namespace {TemplateName}.", $"namespace {appName}.");
                    newContent = newContent.Replace($"namespace {TemplateName}", $"namespace {appName}");
                    newContent = newContent.Replace($"using {TemplateName}.", $"using {appName}.");
                    newContent = newContent.Replace($"using {TemplateName}", $"using {appName}");
                    newContent = newContent.Replace($"{TemplateName}.API", $"{appName}.API");
                    newContent = newContent.Replace($"{TemplateName}.Application", $"{appName}.Application");
                    newContent = newContent.Replace($"{TemplateName}.Domain", $"{appName}.Domain");
                    newContent = newContent.Replace($"{TemplateName}.Infrastructure", $"{appName}.Infrastructure");
                    newContent = newContent.Replace($"{TemplateName}.Persistence", $"{appName}.Persistence");
                    newContent = newContent.Replace($"{TemplateName}.Identity", $"{appName}.Identity");

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

        // Create a new .gitignore file for the generated project
        CreateGitIgnore(targetDir);

        // Update solution file name and content
        var solutionFiles = Directory.GetFiles(targetDir, "*.sln");
        foreach (var solutionFile in solutionFiles)
        {
            var newSolutionName = Path.GetFileName(solutionFile).Replace(TemplateName, appName);
            var newSolutionPath = Path.Combine(Path.GetDirectoryName(solutionFile), newSolutionName);

            // Read and update solution content
            var solutionContent = File.ReadAllText(solutionFile);
            var updatedSolutionContent = solutionContent.Replace(TemplateName, appName);
            File.WriteAllText(newSolutionPath, updatedSolutionContent);

            // Delete the old solution file if it still exists
            if (solutionFile != newSolutionPath && File.Exists(solutionFile))
            {
                File.Delete(solutionFile);
            }

            LogVerbose($"Created solution: {newSolutionPath}");
        }

        LogVerbose("Template copying completed");
    }

    // Helper method to determine if an item should be excluded
    private bool ShouldExcludeItem(string itemPath, string sourceDir, string[] excludePatterns, string[] excludeFiles)
    {
        var relativePath = GetRelativePath(sourceDir, itemPath);
        var fileName = Path.GetFileName(itemPath);

        // Check if item matches any exclusion pattern
        if (excludePatterns.Any(pattern =>
            relativePath.IndexOf(pattern, StringComparison.OrdinalIgnoreCase) >= 0))
        {
            LogVerbose($"Excluded (pattern): {relativePath}");
            return true;
        }

        // Check if item matches any excluded file name
        if (excludeFiles.Any(file =>
            fileName.Equals(file, StringComparison.OrdinalIgnoreCase)))
        {
            LogVerbose($"Excluded (file): {relativePath}");
            return true;
        }

        // Exclude nupkg files
        if (itemPath.EndsWith(".nupkg", StringComparison.OrdinalIgnoreCase))
        {
            LogVerbose($"Excluded (nupkg): {relativePath}");
            return true;
        }

        return false;
    }

    private void CreateGitIgnore(string targetDir)
    {
        var gitignoreContent = @"
            ## Ignore Visual Studio temporary files, build results, and
            ## files generated by popular Visual Studio add-ons.

            # User-specific files
            *.rsuser
            *.suo
            *.user
            *.userosscache
            *.sln.docstates

            # User-specific files (MonoDevelop/Xamarin Studio)
            *.userprefs

            # Build results
            [Dd]ebug/
            [Dd]ebugPublic/
            [Rr]elease/
            [Rr]eleases/
            x64/
            x86/
            [Ww][Ii][Nn]32/
            [Aa][Rr][Mm]/
            [Aa][Rr][Mm]64/
            bld/
            [Bb]in/
            [Oo]bj/
            [Oo]ut/
            msbuild.log
            msbuild.err
            msbuild.wrn

            # Visual Studio 2015/2017 cache/options directory
            .vs/
            # Uncomment if you have tasks that create the project's static files in wwwroot
            #wwwroot/

            # Visual Studio 2017 auto generated files
            Generated\ Files/

            # MSTest test Results
            [Tt]est[Rr]esult*/
            [Bb]uild[Ll]og.*

            # NUnit
            *.VisualState.xml
            TestResult.xml
            nunit-*.xml

            # Build Results of an ATL Project
            [Dd]ebugPS/
            [Rr]eleasePS/
            dlldata.c

            # Benchmark Results
            BenchmarkDotNet.Artifacts/

            # .NET Core
            project.lock.json
            project.fragment.lock.json
            artifacts/

            # ASP.NET Scaffolding
            ScaffoldingReadMe.txt

            # StyleCop
            StyleCopReport.xml

            # Files built by Visual Studio
            *_i.c
            *_p.c
            *_h.h
            *.ilk
            *.meta
            *.obj
            *.iobj
            *.pch
            *.pdb
            *.ipdb
            *.pgc
            *.pgd
            *.rsp
            *.sbr
            *.tlb
            *.tli
            *.tlh
            *.tmp
            *.tmp_proj
            *_wpftmp.csproj
            *.log
            *.vspscc
            *.vssscc
            .builds
            *.pidb
            *.svclog
            *.scc

            # Chutzpah Test files
            _Chutzpah*

            # Visual C++ cache files
            ipch/
            *.aps
            *.ncb
            *.opendb
            *.opensdf
            *.sdf
            *.cachefile
            *.VC.db
            *.VC.VC.opendb

            # Visual Studio profiler
            *.psess
            *.vsp
            *.vspx
            *.sap

            # Visual Studio Trace Files
            *.e2e

            # TFS 2012 Local Workspace
            $tf/

            # Guidance Automation Toolkit
            *.gpState

            # ReSharper is a .NET coding add-in
            _ReSharper*/
            *.[Rr]e[Ss]harper
            *.DotSettings.user

            # TeamCity is a build add-in
            _TeamCity*

            # DotCover is a Code Coverage Tool
            *.dotCover

            # AxoCover is a Code Coverage Tool
            .axoCover/*
            !.axoCover/settings.json

            # Coverlet is a free, cross platform Code Coverage Tool
            coverage*.json
            coverage*.xml
            coverage*.info

            # Visual Studio code coverage results
            *.coverage
            *.coveragexml

            # NCrunch
            _NCrunch_*
            .*crunch*.local.xml
            nCrunchTemp_*

            # MightyMoose
            *.mm.*
            AutoTest.Net/

            # Web workbench (sass)
            .sass-cache/

            # Installshield output folder
            [Ee]xpress/

            # DocProject is a documentation generator add-in
            DocProject/buildhelp/
            DocProject/Help/*.HxT
            DocProject/Help/*.HxC
            DocProject/Help/*.hhc
            DocProject/Help/*.hhk
            DocProject/Help/*.hhp
            DocProject/Help/Html2
            DocProject/Help/html

            # Click-Once directory
            publish/

            # Publish Web Output
            *.[Pp]ublish.xml
            *.azurePubxml
            # Note: Comment the next line if you want to checkin your web deploy settings,
            # but database connection strings (with potential passwords) will be unencrypted
            *.pubxml
            *.publishproj

            # Microsoft Azure Web App publish settings. Comment the next line if you want to
            # checkin your Azure Web App publish settings, but sensitive information contained
            # in these conf files will be unencrypted
            PublishScripts/

            # NuGet Packages
            *.nupkg
            # NuGet Symbol Packages
            *.snupkg
            # The packages folder can be ignored because of Package Restore
            **/[Pp]ackages/*
            # except build/, which is used as an MSBuild target.
            !**/[Pp]ackages/build/
            # Uncomment if necessary however generally it will be regenerated when needed
            #!**/[Pp]ackages/repositories.config
            # NuGet v3's project.json files produces more ignorable files
            *.nuget.props
            *.nuget.targets

            # Microsoft Azure Build Output
            csx/
            *.build.csdef

            # Microsoft Azure Emulator
            ecf/
            rcf/

            # Windows Store app package directories and files
            AppPackages/
            BundleArtifacts/
            Package.StoreAssociation.xml
            _pkginfo.txt
            *.appx
            *.appxbundle
            *.appxupload

            # Visual Studio cache files
            # files ending in .cache can be ignored
            *.[Cc]ache
            # but keep track of directories ending in .cache
            !?*.[Cc]ache/

            # Others
            ClientBin/
            ~$*
            *~
            *.dbmdl
            *.dbproj.schemaview
            *.jfm
            *.pfx
            *.publishsettings
            orleans.codegen.cs

            # Including strong name files can present a security risk
            # (https://github.com/dotnet/core/issues/2621)
            #*.snk

            # Since there are multiple workflows, uncomment next line to ignore bower_components
            # (https://github.com/github/gitignore/pull/1529#issuecomment-104372622)
            #bower_components/

            # RIA/Silverlight projects
            Generated_Code/

            # Backup & report files from converting an old project file
            # to a newer Visual Studio version. Backup files are not needed,
            # because we have git ;-)
            _UpgradeReport_Files/
            Backup*/
            UpgradeLog*.XML
            UpgradeLog*.htm
            ServiceFabricBackup/
            *.rptproj.bak

            # SQL Server files
            *.mdf
            *.ldf
            *.ndf

            # Business Intelligence projects
            *.rdl.data
            *.bim.layout
            *.bim_*.settings
            *.rptproj.rsuser
            *- [Bb]ackup.rdl
            *- [Bb]ackup ([0-9]).rdl
            *- [Bb]ackup ([0-9][0-9]).rdl

            # Microsoft Fakes
            FakesAssemblies/

            # GhostDoc plugin setting file
            *.GhostDoc.xml

            # Node.js Tools for Visual Studio
            .ntvs_analysis.dat
            node_modules/

            # Visual Studio 6 build log
            *.plg

            # Visual Studio 6 workspace options file
            *.opt

            # Visual Studio 6 auto-generated workspace file (contains which files were open etc.)
            *.vbw

            # Visual Studio LightSwitch build output
            **/*.HTMLClient/GeneratedArtifacts
            **/*.DesktopClient/GeneratedArtifacts
            **/*.DesktopClient/ModelManifest.xml
            **/*.Server/GeneratedArtifacts
            **/*.Server/ModelManifest.xml
            _Pvt_Extensions

            # Paket dependency manager
            .paket/paket.exe
            paket-files/

            # FAKE - F# Make
            .fake/

            # CodeRush personal settings
            .cr/personal

            # Python Tools for Visual Studio (PTVS)
            __pycache__/
            *.pyc

            # Cake - Uncomment if you are using it
            # tools/**
            # !tools/packages.config

            # Tabs Studio
            *.tss

            # Telerik's JustMock configuration file
            *.jmconfig

            # BizTalk build output
            *.btp.cs
            *.btm.cs
            *.odx.cs
            *.xsd.cs

            # OpenCover UI analysis results
            OpenCover/

            # Azure Stream Analytics local run output
            ASALocalRun/

            # MSBuild Binary and Structured Log
            *.binlog

            # NVidia Nsight GPU debugger configuration file
            *.nvuser

            # MFractors (Xamarin productivity tool) working folder
            .mfractor/

            # Local History for Visual Studio
            .localhistory/

            # BeatPulse healthcheck temp database
            healthchecksdb

            # Backup folder for Package Reference Convert tool in Visual Studio 2017
            MigrationBackup/

            # Ionide (cross platform F# VS Code tools) working folder
            .ionide/

            # Fody - auto-generated XML schema
            FodyWeavers.xsd
            ";

        File.WriteAllText(Path.Combine(targetDir, ".gitignore"), gitignoreContent.Trim());
        LogVerbose("Created .gitignore file");
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