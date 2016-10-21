using System;

namespace TerminologyLauncher.Entities.FileRepository
{
    public class RepositoryItemEntity
    {
        public string ProvideId { get; set; }
        public string Name { get; set; }
        public string Md5 { get; set; }
        public string DownloadPath { get; set; }
    }
}