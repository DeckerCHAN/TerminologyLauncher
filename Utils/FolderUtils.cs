using System;
using System.IO;

namespace TerminologyLauncher.Auth.Utils
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
    }
}
