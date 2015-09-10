using System;
using System.IO;
using System.Windows.Forms;
using TerminologyLauncher.Core;

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
