using System;
using System.IO;
using System.Reflection;

namespace TerminologyLauncher.Utils
{
    public static class ResourceUtils
    {
        public static Stream ReadEmbedFileResource(String link)
        {
            var currentAssembly = Assembly.GetCallingAssembly();
            var stream = currentAssembly.GetManifestResourceStream(link);
            return stream;
        }

        public static void CopyEmbedFileResource(String link, FileInfo file)
        {
            if (file.Exists)
            {
                file.Delete();
            }

            var currentAssembly = Assembly.GetCallingAssembly();
            using (var resourceStream = currentAssembly.GetManifestResourceStream(link))
            {
                if (resourceStream == null)
                {
                    throw new FileNotFoundException(String.Format("Link {0} is not invalid, can not find that file.", link));
                }
                using (var fileStream = file.OpenWrite())
                {
                    resourceStream.Seek(0, SeekOrigin.Begin);
                    resourceStream.CopyTo(fileStream);
                    fileStream.Flush();
                }
            }



        }
    }
}
