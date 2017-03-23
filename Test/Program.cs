using System;
using System.IO;
using System.Text;
using TerminologyLauncher.GUI;
using TerminologyLauncher.Utils.SerializeUtils;
using InstanceCreateWindow = TerminologyLauncher.GUI.Windows.InstanceCreateWindows.InstanceCreateWindow;

namespace TerminologyLauncher.Test
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            InstanceCreateWindow window = new InstanceCreateWindow();
            window.Show();
            Console.ReadKey();
        }
    }
}