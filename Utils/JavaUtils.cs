using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TerminologyLauncher.Entities.System.Java;

namespace TerminologyLauncher.Utils
{
    public static class JavaUtils
    {
        public static JavaDetails GetJavaDetails(String javaExePath)
        {
            var javaExeFile = new FileInfo(javaExePath);
            if (!javaExeFile.Exists || !javaExeFile.Name.ToLower().Equals("java.exe"))
            {
                throw new FileNotFoundException(String.Format("{0} is not valid java executable file."));
            }
            var process = new Process { StartInfo = new ProcessStartInfo(javaExeFile.FullName) { Arguments = "-version", CreateNoWindow = true, UseShellExecute = false, RedirectStandardError = true } };
            process.Start();
            process.WaitForExit();

            var content = process.StandardError.ReadToEnd();

            var version = Regex.Match(content, "(?<=java version\\ \\\").*(?=\\\")").Value;
            var bit = Regex.Match(content, "(?<=Java HotSpot\\(TM\\)\\ ).*(?=\\ VM \\(build)").Value;
            if (String.IsNullOrEmpty(version) || String.IsNullOrEmpty(bit))
            {
                throw new Exception("Invalid java exe!");
            }

            var jDetail = new JavaDetails { JavaVersion = version };

            if (bit.EndsWith("Server"))
            {
                jDetail.JavaType = bit.StartsWith("64-Bit") ? JavaType.ServerX64 : JavaType.ServerX86;
            }
            else if (bit.EndsWith("Client"))
            {
                jDetail.JavaType = bit.StartsWith("64-Bit") ? JavaType.ClientX64 : JavaType.ClientX86;
            }
            else
            {
                jDetail.JavaType = JavaType.Unknown;
            }
            return jDetail;
        }

        public static JavaRuntimeEntity GetJavaRuntimeFromBinFolder(String javaBinFolderPath)
        {
            var javaBinFolder = new DirectoryInfo(javaBinFolderPath);
            if (!javaBinFolder.Exists)
            {
                throw new DirectoryNotFoundException("Java bin folder not exists!");
            }

            var javaExeFile = new FileInfo(Path.Combine(javaBinFolder.FullName, "java.exe"));
            var javawExeFile = new FileInfo(Path.Combine(javaBinFolder.FullName, "javaw.exe"));

            if (!javaExeFile.Exists || !javawExeFile.Exists)
            {
                throw new FileNotFoundException("Java file or javaw file not exists!");
            }

            return new JavaRuntimeEntity
            {
                JavaDetails = GetJavaDetails(javaExeFile.FullName),
                JavaPath = javaExeFile.FullName,
                JavaWPath = javawExeFile.FullName
            };
        }
    }
}
