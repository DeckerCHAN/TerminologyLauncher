using System;
using System.IO;
using TerminologyLauncher.Configs;

namespace TerminologyLauncher.FileRepository
{
    public class FileRepository
    {
        public String QueryUrl { get; set; }
        public Config Config { get; set; }
        public FileRepository()
        {
            this.Config = new Config(new FileInfo("Configs/FileRepositoryConfig.json"));
            this.QueryUrl = this.Config.GetConfig("FileRepositoryUpdateUrl");
        }
    }
}
