using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TerminologyLauncher.GUI;
using TerminologyLauncher.Utils.ProgressService;

namespace Test
{
    public class ProgressWindowTest
    {
        public static void MainTest()
        {
            var progress = new LeafNodeProgress("Common");
            ProgressWindow progressWindow = new ProgressWindow(progress);

            var t = Task.Run(() =>
            {
                do
                {
                    Thread.Sleep(1000);
                    progress.Percent += 5.5D;
                    Console.WriteLine("Progress update to {0}!", progress.Percent);
                } while (true);
            });

            progressWindow.ShowDialog();

            //var t = Task.Run(() =>
            //{
            //    progressWindow = new ProgressWindow(progress);
            //    progressWindow.Show();
            //    Console.WriteLine("Show progress!");
            //});



        }

    }
}
