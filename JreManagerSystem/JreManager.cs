using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TerminologyLauncher.Configs;
using TerminologyLauncher.Entities.System.Java;
using TerminologyLauncher.Utils;

namespace TerminologyLauncher.JreManagerSystem
{
    public class JreManager
    {
        private Dictionary<String, JavaRuntimeEntity> AvailableJavaRuntimeValue;
        public Config Config { get; set; }

        public JavaRuntimeEntity JavaRuntime
        {
            get
            {
                var je = this.Config.GetConfigObject<JavaRuntimeEntity>("usingJre");
                return JavaUtils.IsJavaRuntimeValid(je) ? je : null;
            }
            set
            {
                if (JavaUtils.IsJavaRuntimeValid(value))
                {
                    this.Config.SetConfigObject("usingJre", value);
                }
                else
                {
                    throw new InvalidOperationException("Try to set an invalid jre!");
                }
            }
        }

        public Dictionary<String, JavaRuntimeEntity> AvailableJavaRuntimes
        {
            get
            {
                return this.AvailableJavaRuntimeValue ?? (this.AvailableJavaRuntimeValue = new Dictionary<string, JavaRuntimeEntity>());
            }
            private set { this.AvailableJavaRuntimeValue = value; }
        }

        public List<String> SearchPaths { get; private set; }

        public JreManager(String configPath)
        {
            this.Config = new Config(new FileInfo(configPath));
            this.SearchPaths = this.Config.GetConfigObject<List<String>>("javaSearchPaths");


            var javaPaths = this.SearchPaths.Where(Directory.Exists)
                .SelectMany(
                path => Directory.GetDirectories(path, "*", SearchOption.TopDirectoryOnly),
                (path, folder) => Path.Combine(folder, "bin/java.exe")
                ).Where(File.Exists)
                .ToList();

            foreach (var javaPath in javaPaths)
            {
                try
                {
                    var runtimeEntity = new JavaRuntimeEntity()
                    {
                        JavaPath = javaPath,
                        JavaWPath = Path.Combine(Path.GetDirectoryName(javaPath), "javaw.exe"),
                        JavaDetails = JavaUtils.GetJavaDetails(javaPath)
                    };
                    this.AvailableJavaRuntimes.Add(String.Format("{0}:{1}", runtimeEntity.JavaDetails.JavaVersion,
                        runtimeEntity.JavaDetails.JavaType), runtimeEntity);
                }
                catch (Exception ex)
                {
                    Logging.Logger.GetLogger().WarnFormat("Can not indentify this java caused by {0}. Ignored!", ex.Message);
                }
            }
        }



    }
}
