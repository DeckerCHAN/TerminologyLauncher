using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Win32;
using TerminologyLauncher.Core;

namespace TerminologyLauncher
{
    public static class Beginner
    {
        public static void Start()
        {
            if (!RuntimeDotNetHigherThan45())
            {
                Process.Start("http://www.microsoft.com/en-us/download/details.aspx?id=30653");
                Console.WriteLine("Current .net version is lower than 4.5! Please download the higher version.");
                return;
            }

            try
            {
                if ((Process.GetProcessesByName("TerminologyLauncher").Length + Process.GetProcessesByName("TerminologyLauncher[DEBUG]").Length) > 1)
                {
                    Console.WriteLine("You can not run nore than one Terminology Launcher at same time!");
                    return;
                }
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Engine.GetEngine().Run();
            }
            catch (Exception ex)
            {
                Directory.CreateDirectory("Crash-report");
                var report = new FileInfo(String.Format("Crash-report\\crash-report-{0}.report", DateTime.Now.ToString("yyyy-MM-dd_hh-mm-ss-tt")));
                File.WriteAllText(report.FullName, ex.ToString());
                Console.WriteLine("!!!CRASH!!!Encountered unhandled exception. System crashed!");
                Console.WriteLine("!!!CRASH!!!More detail at {0}", report.FullName);
            }
        }

        private static Boolean RuntimeDotNetHigherThan45()
        {
            using (RegistryKey ndpKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32).OpenSubKey("SOFTWARE\\Microsoft\\NET Framework Setup\\NDP\\v4\\Full\\"))
            {
                int releaseKey = Convert.ToInt32(ndpKey.GetValue("Release"));
                return releaseKey >= 378389;
            }
        }

    }
}
