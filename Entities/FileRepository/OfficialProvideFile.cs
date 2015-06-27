using System;

namespace TerminologyLauncher.Entities.FileRepository
{
    public class OfficialProvideFile
    {
        public String Id { get; set; }
        public String FileName { get; set; }
        public String Md5 { get; set; }
        public String DownloadPath { get; set; }
    }
}
