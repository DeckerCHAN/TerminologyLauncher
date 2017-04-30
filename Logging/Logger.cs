using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading;
using log4net;
using log4net.Appender;
using log4net.Config;
using log4net.Repository.Hierarchy;
using TerminologyLauncher.Utils;

namespace TerminologyLauncher.Logging
{
    public static class TerminologyLogger
    {
        private static bool _isLoaded = false;

        private static MemoryAppender MemoryAppender;

        [MethodImpl(MethodImplOptions.Synchronized)]
        public  static ILog GetLogger()
        {
            if (!_isLoaded)
            {
                XmlConfigurator.Configure(
                    ResourceUtils.ReadEmbedFileResource("TerminologyLauncher.Logging.Configs.Logging.config"));
                var hierarchy = LogManager.GetRepository() as Hierarchy;
                if (hierarchy != null) MemoryAppender = hierarchy.Root.GetAppender("MemoryAppender") as MemoryAppender;

                _isLoaded = true;
            }

            var logger = LogManager.GetLogger(new StackTrace().GetFrame(1).GetMethod().ReflectedType);;
            return logger;
        }

        public static string GetLogConent()
        {
            if (MemoryAppender == null)
            {
                return string.Empty;
            }

            using (var content = new StringWriter())
            {
                foreach (var loggingEvent in MemoryAppender.GetEvents())
                {
                    MemoryAppender.Layout.Format(content, loggingEvent);
                    content.Write(Environment.NewLine);
                }
                return content.ToString();
            }
        }
    }
}