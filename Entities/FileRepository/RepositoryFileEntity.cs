using System;

namespace TerminologyLauncher.Entities.FileRepository
{
    public class RepositoryFileEntity
    {
        public String ProvideId { get; set; }
        public String Name { get; set; }
        public String Md5 { get; set; }
        public String DownloadPath { get; set; }
    }
}
