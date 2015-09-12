using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TerminologyLauncher.Entities.InstanceManagement
{
    public class InstanceStartupArgumentsEntity
    {
        public List<String> JvmArguments { get; set; }
        public List<String> LibraryPathes { get; set; }
        public String MainJarPath { get; set; }
        public String MainClass { get; set; }
        public String Version { get; set; }
        public String AssetsDir { get; set; }
        public String AssetIndex { get; set; }
        public String UserProperties { get; set; }
        public String UserType { get; set; }
        public List<String> TweakClass { get; set; }

    }
}
