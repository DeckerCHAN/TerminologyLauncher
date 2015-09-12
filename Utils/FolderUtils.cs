using System;
using System.IO;
using System.Reflection;

namespace TerminologyLauncher.Utils
{
    public static class FolderUtils
    {
        public static readonly DirectoryInfo SystemTempFolder = new DirectoryInfo(Path.GetTempPath());
        public static readonly DirectoryInfo ExecutorFolder = new DirectoryInfo(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location));
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

        public static void DeleteDirectory(string path)
        {
            foreach (var directory in Directory.GetDirectories(path))
            {
                DeleteDirectory(directory);
            }
            try
            {
                Directory.Delete(path, true);
            }
            catch (IOException)
            {
                Directory.Delete(path, true);
            }
            catch (UnauthorizedAccessException)
            {
                Directory.Delete(path, true);
            }
        }
    }
}
