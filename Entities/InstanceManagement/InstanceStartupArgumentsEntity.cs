using System;
using System.Collections.Generic;

namespace TerminologyLauncher.Entities.InstanceManagement
{
    public class InstanceStartupArgumentsEntity
    {
        public List<string> JvmArguments { get; set; }
        public long MiniumMemoryMegaSize { get; set; }
        public string Nativespath { get; set; }
        public List<string> LibraryPaths { get; set; }
        public string MainJarPath { get; set; }
        public string MainClass { get; set; }
        public string Version { get; set; }
        public string AssetsDir { get; set; }
        public string AssetIndex { get; set; }
        public string UserProperties { get; set; }
        public string UserType { get; set; }
        public List<string> TweakClasses { get; set; }

    }
}
