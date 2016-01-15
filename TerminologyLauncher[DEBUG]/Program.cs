using System;

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
            Console.WriteLine("Programme returned...Press any key to exit.");
            Console.ReadKey();
            Environment.Exit(0);

        }
    }
}
