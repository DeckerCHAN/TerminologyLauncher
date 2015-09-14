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
            var result = new UiControl().StartSingleSelect("Test", "TestField", new List<String> { "asdasd", "dasdddd" });
            
        }
    }
}
