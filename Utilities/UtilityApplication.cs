using ConsoleIO;

namespace Utilities
{
    internal class UtilityApplication
    {
        public void Run()
        {
            // Test the ConsoleIO library
            using (var menu = new ConsoleMenu(settings =>
            {
                settings.Title = "Test Menu";
                settings.Background = ConsoleColor.DarkBlue;
                settings.Foreground = ConsoleColor.White;
            })
            .AddMessage("Hello world!", settings =>
            {
                settings.Background = ConsoleColor.DarkGreen;
                settings.Foreground = ConsoleColor.White;
            })
            .AddCheckbox("I agree to the terms and conditions", out var agreeToTerms, 
            settings =>
            {
                settings.Checked = false;
                settings.Foreground = ConsoleColor.Red;
            })
            .AddOption("Confirm", settings =>
            {
                settings.Enabled = false;
            })
            .Enter()) { }

            Console.WriteLine($"{Environment.UserName} @ {Environment.MachineName}");

            if (TryGetSolutionDirectoryInfo() is DirectoryInfo directory)
            {
                Console.WriteLine($"Solution: {directory.Name} @ {directory.FullName}");

                GetProjects(directory).ForEach(project =>
                {
                    Console.WriteLine($"Project: {project.Name} @ {project.FullName}");
                    GetFolders(project).ForEach(folder =>
                    {
                        Console.WriteLine($" Folder: {folder.Name} @ {folder.FullName}");
                        GetFiles(folder).ForEach(file =>
                        {
                            Console.WriteLine($"  File: {file.Name} @ {file.FullName}");
                        });
                    });
                });
            }

            Console.WriteLine("Hello utils!");
            Console.ReadKey();
        }

        public static DirectoryInfo? TryGetSolutionDirectoryInfo(string? currentPath = null)
        {
            var directory = new DirectoryInfo(currentPath ?? Directory.GetCurrentDirectory());

            while (directory is not null && !directory.GetFiles("*.sln").Any())
                directory = directory.Parent;

            return directory;
        }

        public static List<DirectoryInfo> GetProjects(DirectoryInfo solutionDirectoryInfo)
        {
            var projects = new List<DirectoryInfo>();

            foreach (var directory in solutionDirectoryInfo.GetDirectories())
            {
                if (directory.GetFiles("*.csproj").Any())
                    projects.Add(directory);
                else
                    projects.AddRange(GetProjects(directory));
            }

            return projects;
        }

        public static List<DirectoryInfo> GetFolders(DirectoryInfo directoryInfo, bool recursive = false)
        {
            var folders = new List<DirectoryInfo>();

            foreach (var directory in directoryInfo.GetDirectories())
            {
                if (directory.Name == "bin" || directory.Name == "obj")
                    continue;

                folders.Add(directory);
                if (recursive) folders.AddRange(GetFolders(directory));
            }

            return folders;
        }

        public static List<FileInfo> GetFiles(DirectoryInfo directoryInfo, bool recursive = false)
        {
            var files = new List<FileInfo>();

            foreach (var file in directoryInfo.GetFiles())
            {
                if (file.Extension == ".cs") files.Add(file);
            }

            if (recursive)
            {
                foreach (var directory in directoryInfo.GetDirectories())
                {
                    files.AddRange(GetFiles(directory));
                }
            }

            return files;
        }

    }
}