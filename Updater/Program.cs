using System;
using System.IO;
using System.Linq;

namespace TerminologyLauncher.Updater
{
    class Program
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
