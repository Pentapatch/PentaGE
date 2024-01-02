using ConsoleIO;

namespace Utilities
{
    internal class UtilityApplication
    {

        // Test the ConsoleIO library
        //ConsoleMenu menu = null!;
        //MenuCheck agreeToTerms = null!;
        //MenuOption confirm = null!;

        //using (ConsoleMenu.Create(out menu,
        //    settings =>
        //    {
        //        settings.Title = "Test Menu";
        //        settings.Background = ConsoleColor.DarkBlue;
        //        settings.Foreground = ConsoleColor.White;
        //    })
        //.AddMessage("Please agree to the terms and conditions:",
        //    out var message,
        //    settings =>
        //    {
        //        settings.Background = ConsoleColor.DarkGreen;
        //        settings.Foreground = ConsoleColor.White;
        //    })
        //.AddCheckbox("I agree to the terms and conditions",
        //    out agreeToTerms,
        //    settings =>
        //    {
        //        settings.Selected = true;
        //        settings.Checked = false;
        //        settings.Foreground = ConsoleColor.Red;
        //    },
        //    action: () =>
        //    {
        //        message.Text = agreeToTerms.Checked
        //            ? "Thank you for agreeing to the terms and conditions."
        //            : "Please agree to the terms and conditions:";
        //        confirm.Enabled = agreeToTerms.Checked;
        //    })
        //.AddOption("Confirm", out confirm, 
        //    settings =>
        //    {
        //        settings.Enabled = false;
        //    },
        //    action: () =>
        //    {
        //        // Perform final action here
        //    })
        //.Enter(
        //    action: () =>
        //    {

        //    })) { }

        private void ExecuteOne()
        {

        }

        public void TestMenu()
        {
            ConsoleMenu.Create()
                .AddMessage("Alternativ:", 
                    setting => setting.Foreground = ConsoleColor.Red)
                .AddCheckbox("Checkbox 1")
                .AddMessage("Meny:")
                .AddOption("Option 1")
                .AddOption("Option 2")
                .Enter();
        }

        public void Run()
        {
            var menu = ConsoleMenu.Create();
            menu.AddMessage($"{Environment.UserName} @ {Environment.MachineName}", x => x.Foreground = ConsoleColor.Red);

            if (TryGetSolutionDirectoryInfo() is DirectoryInfo directory)
            {
                menu.AddMessage($"Solution: {directory.Name} @ {directory.FullName}", x => x.Foreground = ConsoleColor.Blue);

                GetProjects(directory).ForEach(project =>
                {
                    menu.AddMessage($"Project: {project.Name} @ {project.FullName}", x => x.Foreground = ConsoleColor.DarkCyan);
                    GetFolders(project).ForEach(folder =>
                    {
                        menu.AddMessage($" Folder: {folder.Name} @ {folder.FullName}", x => x.Foreground = ConsoleColor.DarkMagenta);
                        GetFiles(folder).ForEach(file =>
                        {
                            menu.AddOption($"  File: {file.Name} @ {file.FullName}");
                        });
                    });
                });
            }

            menu.Enter();

            //TestMenu();

            //ConsoleMenu.Create(settings =>
            //    {
            //        settings.Background = ConsoleColor.DarkBlue;
            //        settings.Foreground = ConsoleColor.Red;
            //        settings.Title = "hehehe";
            //        settings.WrapArround = false;
            //    })
            //.AddOption("Option 1", ExecuteOne)
            //.AddOption("Option 2",
            //    settings =>
            //    {
            //        settings.Selected = true;
            //    })
            //.AddOption("Option 3", Console.Clear)
            //.Enter();

            //MenuCheck check = null!;
            //MenuOption option3 = null!;
            //using (ConsoleMenu.Create(
            //    settings =>
            //    {
            //        settings.Title = "Test Menu";
            //    })
            //    //.SetDisabledBackground(ConsoleColor.DarkRed)
            //    .SetTitle("Dennis konsoll")
            //    .SetMessageForeground(ConsoleColor.Blue)
            //    .AddMessage("Welcome to the simple test menu!",
            //        settings =>
            //        {
            //            settings.Foreground = ConsoleColor.Gray;
            //        })
            //    .AddCheckbox("Enable all",
            //        out check,
            //        settings =>
            //        {
            //            settings.Shortcut = ConsoleKey.F5;
            //        },
            //        action: () =>
            //        {
            //            Console.Title = check.Checked ? "HELLO WORLD!" : "...";
            //            option3.Enabled = check.Checked;
            //        })
            //    .AddMessage("Section one:")
            //    .AddOption("Option 1")
            //    .AddOption("Option 2",
            //        settings =>
            //        {
            //            settings.Selected = true;
            //        })
            //    .AddOption("Option 3",
            //        out option3,
            //        settings =>
            //        {
            //            settings.Enabled = false;
            //            settings.Shortcut = ConsoleKey.F;
            //        })
            //    .AddMessage("Section two:")
            //    .AddOption("Option 4")
            //    .AddOption("Option 5")
            //    .Enter()) { }

            //Console.WriteLine($"{Environment.UserName} @ {Environment.MachineName}");

            //if (TryGetSolutionDirectoryInfo() is DirectoryInfo directory)
            //{
            //    Console.WriteLine($"Solution: {directory.Name} @ {directory.FullName}");

            //    GetProjects(directory).ForEach(project =>
            //    {
            //        Console.WriteLine($"Project: {project.Name} @ {project.FullName}");
            //        GetFolders(project).ForEach(folder =>
            //        {
            //            Console.WriteLine($" Folder: {folder.Name} @ {folder.FullName}");
            //            GetFiles(folder).ForEach(file =>
            //            {
            //                Console.WriteLine($"  File: {file.Name} @ {file.FullName}");
            //            });
            //        });
            //    });
            //}

            //Console.WriteLine("Hello utils!");
            //Console.ReadKey();
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