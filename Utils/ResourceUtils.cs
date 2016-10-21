using System;
using System.IO;
using System.Reflection;

namespace TerminologyLauncher.Utils
{
    public static class ResourceUtils
    {
        public static string ReadEmbedFileAsString(string link)
        {
            var stream = ReadEmbedFileResource(Assembly.GetCallingAssembly(), link);
            var result = string.Empty;
            using (var reader = new StreamReader(stream))
            {
                result = reader.ReadToEnd();
            }
            return result;
        }
        public static Stream ReadEmbedFileResource(string link)
        {
            var currentAssembly = Assembly.GetCallingAssembly();
            var stream = currentAssembly.GetManifestResourceStream(link);
            return stream;
        }

        public static Stream ReadEmbedFileResource(Assembly assembly,string link)
        {
            var stream = assembly.GetManifestResourceStream(link);
            return stream;
        }

        public static void CopyEmbedFileResource(string link, FileInfo file)
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
                    throw new FileNotFoundException($"Link {link} is not invalid, cannot find that file.");
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
