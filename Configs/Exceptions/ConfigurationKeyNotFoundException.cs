using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using TerminologyLauncher.Utils.Exceptions;

namespace TerminologyLauncher.Configs.Exceptions
{
    public sealed class ConfigurationKeyNotFoundException : SolutionProvidedException
    {
        public String MissingKey { get; private set; }
        public String Config { get; private set; }

        public ConfigurationKeyNotFoundException(String config, String missingConfigKey)
            : base(String.Format("Key {0} for {1} is missing.", missingConfigKey, config), "Re-install may required to resolve this problem")
        {
            this.Config = config;
            this.MissingKey = missingConfigKey;
        }


    }
}
