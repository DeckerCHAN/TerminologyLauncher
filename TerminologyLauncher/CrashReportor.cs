using System;
using System.IO;
using System.Net;
using System.Text;
using TerminologyLauncher.Logging;

namespace TerminologyLauncher
{
    public class CrashReportor
    {
        public String Report
        {
            get
            {
                var reportBuilder = new StringBuilder();
                reportBuilder.AppendLine("Crash report:");
                reportBuilder.AppendLine(String.Format("Data:{0}", DateTime.Now.ToString("yyyy-MM-dd_hh-mm-ss-tt")));
                reportBuilder.AppendLine(String.Format("Detail:{0}", this.Exception));
                return reportBuilder.ToString();
            }
        }

        public Exception Exception { get; set; }
        public String Log { get; set; }
        public String ReportUrl { get { return "http://termlauncher.applinzi.com/report"; } }
        public FileInfo ReportFileInfo { get { return new FileInfo(String.Format("Crash-report\\crash-report-{0}.report", DateTime.Now.ToString("yyyy-MM-dd_hh-mm-ss-tt"))); } }
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
                var resp = webClient.UploadString(this.ReportUrl, String.Format("crash_report={0}&log={1}", Uri.EscapeDataString(this.Report),
                    Uri.EscapeDataString(this.Log)));
                Console.WriteLine(resp);

            }


        }
    }
}
