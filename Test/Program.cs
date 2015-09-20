using System;
using System.Collections.Generic;
using TerminologyLauncher.GUI;
using TerminologyLauncher.GUI.ToolkitWindows.SingleSelect;

namespace TerminologyLauncher.Test
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            Console.WriteLine(Utils.MachineUtils.GetTotalMemoryInMiB());
            Console.ReadKey();
        }
    }
}
