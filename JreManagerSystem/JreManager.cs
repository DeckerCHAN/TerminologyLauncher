using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TerminologyLauncher.Configs;

namespace TerminologyLauncher.JreManagerSystem
{
    public class JreManager
    {
        public Config Config { get; set; }

        public JreManager(String configPath)
        {
            this.Config =new Config(new FileInfo(configPath));
        }

    }
}
