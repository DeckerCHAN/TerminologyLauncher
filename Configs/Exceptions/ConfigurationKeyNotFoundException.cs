using System;
using TerminologyLauncher.Utils.Exceptions;

namespace TerminologyLauncher.Configs.Exceptions
{
    public sealed class ConfigurationKeyNotFoundException : SolutionProvidedException
    {
        public string MissingKey { get; private set; }
        public string Config { get; private set; }

        public ConfigurationKeyNotFoundException(string config, string missingConfigKey)
            : base($"Key {missingConfigKey} for {config} is missing.", "Re-install may required to resolve this problem")
        {
            this.Config = config;
            this.MissingKey = missingConfigKey;
        }


    }
}
