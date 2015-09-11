using System;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace TerminologyLauncher.Updater
{
    static class Program
    {
        static void Main(string[] args)
        {
            if (args.Length != 2)
            {
                Console.WriteLine("Wrong number of arguments! Exit...");
                return;
            }

            if (!(Directory.Exists(args[0]) && Directory.Exists(args[1])))
            {
                Console.WriteLine("Source folder or target folder not exists! Exit...");
            }
            Console.WriteLine("Searching for TerminologyLauncher process and wait for close!");
            foreach (var process in Process.GetProcessesByName("TerminologyLauncher"))
            {
                process.WaitForExit();
            }

            if ((+ Process.GetProcessesByName("TerminologyLauncher[DEBUG]").Length) > 1)
            {
                Console.WriteLine("You can not run nore than one Terminology Launcher at same time!");
                return;
            }

            var sourceFolder = new DirectoryInfo(args[0]);
            var targetFolder = new DirectoryInfo(args[1]);

            CopyAllFilesToDirectory(sourceFolder.FullName, targetFolder.FullName);
            Console.WriteLine("Done...Press any key to continue...");
            Console.ReadKey();

        }

        public static void CopyAllFilesToDirectory(String sourceDir, String targetDir)
        {
            Directory.CreateDirectory(targetDir);

            foreach (var file in Directory.GetFiles(sourceDir))
            {
                File.Copy(file, Path.Combine(targetDir, Path.GetFileName(file)));
                Console.WriteLine("Copied {0} from {1} to {2}", Path.GetFileName(file), sourceDir, targetDir);
            }

            foreach (var directory in Directory.GetDirectories(sourceDir))
            {
                CopyAllFilesToDirectory(directory, Path.Combine(targetDir, Path.GetFileName(directory)));
                Console.WriteLine("Copied {0} from {1} to {2}", Path.GetFileName(directory), sourceDir, targetDir);
            }
        }
    }
}
