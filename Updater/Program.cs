﻿using System;
using System.Diagnostics;
using System.IO;

namespace TerminologyLauncher.Updater
{
    static class Program
    {
        static void Main(string[] args)
        {
            try
            {
                if (args.Length != 3)
                {
                    Console.WriteLine("Wrong number of arguments! Exit...");
                    return;
                }

                if (!(Directory.Exists(args[0]) && Directory.Exists(args[1])))
                {
                    Console.WriteLine("Source folder or target folder not exists! Exit...");
                }
                if ((Process.GetProcessesByName("TerminologyLauncher").Length +
                     Process.GetProcessesByName("TerminologyLauncher[DEBUG]").Length) > 0)
                {
                    Console.WriteLine("Searching for TerminologyLauncher process and wait for close!");
                    foreach (var process in Process.GetProcessesByName("TerminologyLauncher"))
                    {
                        process.WaitForExit();
                    }
                    foreach (var process in Process.GetProcessesByName("TerminologyLauncher[DEBUG]"))
                    {
                        process.WaitForExit();
                    }
                }


                var sourceFolder = new DirectoryInfo(args[0]);
                var targetFolder = new DirectoryInfo(args[1]);

                CopyAllFilesToDirectory(sourceFolder.FullName, targetFolder.FullName);

                var newProcess = new Process { StartInfo = { FileName = args[2] } };
                newProcess.Start();

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
       
            Console.WriteLine("Done...Press any key to continue...");
        }

        public static void CopyAllFilesToDirectory(string sourceDir, string targetDir)
        {
            Directory.CreateDirectory(targetDir);

            foreach (var file in Directory.GetFiles(sourceDir))
            {
                File.Copy(file, Path.Combine(targetDir, Path.GetFileName(file)), true);
                Console.WriteLine($"Copied {Path.GetFileName(file)} from {sourceDir} to {targetDir}");
            }

            foreach (var directory in Directory.GetDirectories(sourceDir))
            {
                CopyAllFilesToDirectory(directory, Path.Combine(targetDir, Path.GetFileName(directory)));
                Console.WriteLine($"Copied {Path.GetFileName(directory)} from {sourceDir} to {targetDir}");
            }
        }
    }
}