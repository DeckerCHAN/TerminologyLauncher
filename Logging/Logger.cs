using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using log4net;

namespace Logging
{

        public static class Logger
        {
            public static ILog GetLogger()
            {
                return LogManager.GetLogger(new StackTrace().GetFrame(1).GetMethod().ReflectedType);
            }
        }
    
}
