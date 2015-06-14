using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
namespace TerminologyLauncher
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            try
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Core.Engine.GetEngine().Run();
            }
            catch (Exception ex)
            {
                FileInfo report =new FileInfo(String.Format("crash-report\\crash-report-{0}.report",DateTime.Now.ToString("yyyy-MM-dd_hh-mm-ss-tt")));
                File.WriteAllText(report.FullName, ex.ToString());
                Console.WriteLine("!!!CRASH!!!Encountered unhandled exception. System crashed!");
                Console.WriteLine("!!!CRASH!!!More detail at {0}", report.FullName);

            }
            finally
            {
                Console.WriteLine("Programme returned...Press any key to exit.");
                Console.ReadKey();
                Environment.Exit(0);
            }

        }
    }
}
