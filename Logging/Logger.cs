using System.Diagnostics;
using log4net;

namespace TerminologyLauncher.Logging
{

        public static class Logger
        {
            public static ILog GetLogger()
            {
                return LogManager.GetLogger(new StackTrace().GetFrame(1).GetMethod().ReflectedType);
            }
        }
    
}
