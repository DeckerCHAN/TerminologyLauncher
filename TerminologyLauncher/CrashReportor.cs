using System;
using System.IO;
using System.Net;
using System.Text;
using TerminologyLauncher.Logging;

namespace TerminologyLauncher
{
    public class CrashReportor
    {
        public string Report
        {
            get
            {
                var reportBuilder = new StringBuilder();
                reportBuilder.AppendLine("Crash report:");
                reportBuilder.AppendLine($"Data:{DateTime.Now.ToString("yyyy-MM-dd_hh-mm-ss-tt")}");
                reportBuilder.AppendLine($"Detail:{this.Exception}");
                return reportBuilder.ToString();
            }
        }

        public Exception Exception { get; set; }
        public string Log { get; set; }
        public string ReportUrl { get { return "http://termlauncher.applinzi.com/report"; } }
        public FileInfo ReportFileInfo { get { return new FileInfo(
            $"Crash-report\\crash-report-{DateTime.Now.ToString("yyyy-MM-dd_hh-mm-ss-tt")}.report"); } }
        public CrashReportor(Exception ex)
        {
            this.Exception = ex;
            this.Log = TerminologyLogger.GetLogConent();
        }

        public void DoReport()
        {
            Console.WriteLine("Creating crash report file...");
            File.WriteAllText(this.ReportFileInfo.FullName, this.Report);

            Console.WriteLine("Report crash to Launcher Report Service...");
            using (var webClient = new WebClient())
            {
                webClient.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
                var resp = webClient.UploadString(this.ReportUrl,
                    $"crash_report={Uri.EscapeDataString(this.Report)}&log={Uri.EscapeDataString(this.Log)}");
                Console.WriteLine(resp);

            }


        }
    }
}
