using System;
using System.IO;

namespace TerminologyLauncher.Utils
{
    public static class FolderUtils
    {
        public static void RecreateFolder(String path)
        {
            RecreateFolder(new DirectoryInfo(path));
        }
        public static void RecreateFolder(DirectoryInfo directoryInfo)
        {
            if (!directoryInfo.Exists)
            {
                directoryInfo.Create();
            }
            else
            {
                directoryInfo.Delete(true);
                directoryInfo.Create();
            }
        }

        public static void DeleteDirectory(string directory)
        {
            string[] files = Directory.GetFiles(directory);
            string[] dirs = Directory.GetDirectories(directory);

            foreach (string file in files)
            {
                File.SetAttributes(file, FileAttributes.Normal);
                File.Delete(file);
            }

            foreach (string dir in dirs)
            {
                DeleteDirectory(dir);
            }

            Directory.Delete(directory, false);
        }
    }
}
