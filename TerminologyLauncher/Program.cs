using System;
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
            Beginner.Start();
            Application.Exit();
        }
    }
}