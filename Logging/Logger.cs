using System;
using System.Diagnostics;
using log4net;
using log4net.Config;
using log4net.Repository.Hierarchy;
using TerminologyLauncher.Utils;

namespace TerminologyLauncher.Logging
{

    public static class TerminologyLogger
    {
        private static Boolean _isLoaded = false;
        public static ILog GetLogger()
        {
            if (_isLoaded) return LogManager.GetLogger(new StackTrace().GetFrame(1).GetMethod().ReflectedType);
            XmlConfigurator.Configure(
                ResourceUtils.ReadEmbedFileResource("TerminologyLauncher.Logging.Configs.Logging.config"));
            _isLoaded = true;
            return LogManager.GetLogger(new StackTrace().GetFrame(1).GetMethod().ReflectedType);
        }
    }

}
