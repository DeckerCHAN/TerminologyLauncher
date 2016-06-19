using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Microsoft.Win32;
using TerminologyLauncher.Core;

namespace TerminologyLauncher
{
    public static class Beginner
    {
        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        public static extern int MessageBox(IntPtr hWnd, string text, string caption, uint type);

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
                    Console.WriteLine("You cannot run nore than one Terminology Launcher at same time!");
                    MessageBox(new IntPtr(0), "You cannot run nore than one Terminology Launcher at same time!", "Launcher already running", 0);
                    return;
                }
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Engine.GetEngine().Run();
            }
            catch (Exception ex)
            {
                Directory.CreateDirectory("Crash-report");
                try
                {
                    Console.WriteLine("!!!CRASH!!!Encountered unhandled exception. System crashed!");
                    Console.WriteLine("Collecting crash report...");
                    var reportor = new CrashReportor(ex);
                    reportor.DoReport();

                    Console.WriteLine($"!!!CRASH!!!More detail at {reportor.ReportFileInfo.FullName}");

                }
                catch (Exception)
                {

                    //ignore.
                }
            }
        }

        private static bool RuntimeDotNetHigherThan45()
        {
            using (RegistryKey ndpKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32).OpenSubKey("SOFTWARE\\Microsoft\\NET Framework Setup\\NDP\\v4\\Full\\"))
            {
                int releaseKey = Convert.ToInt32(ndpKey.GetValue("Release"));
                return releaseKey >= 378389;
            }
        }

    }
}
