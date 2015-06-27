using System;
using System.Collections.Generic;
using System.IO;
using TerminologyLauncher.Configs;
using TerminologyLauncher.Entities.FileRepository;
using TerminologyLauncher.Entities.InstanceManagement;
using TerminologyLauncher.Entities.SerializeUtils;
using TerminologyLauncher.Logging;
using TerminologyLauncher.Utils;

namespace TerminologyLauncher.FileRepository
{
    public class FileRepository
    {
        public String RepoUrl { get; set; }
        public Config Config { get; set; }
        public Dictionary<String, OfficialProvideFile> OfficialProviRdeFilesRepo { get; set; }
        public FileRepository()
        {
            Logger.GetLogger().Info("Initializing file repo...");
            this.Config = new Config(new FileInfo("Configs/FileRepositoryConfig.json"));
            this.RepoUrl = this.Config.GetConfig("FileRepositoryUrl");
            this.OfficialProviRdeFilesRepo = new Dictionary<string, OfficialProvideFile>();
            DownloadUtils.DownloadFile(this.RepoUrl, this.Config.GetConfig("RepoFilePath"));
            var repo = JsonConverter.Parse<FileRepo>(System.IO.File.ReadAllText(this.Config.GetConfig("RepoFilePath")));
            foreach (var officialProvideFile in repo.Files)
            {
                this.OfficialProviRdeFilesRepo.Add(officialProvideFile.Id, officialProvideFile);
            }
            Logger.GetLogger().Info("Initialized file repo!");

        }

        public void GetOfficialFile(String rootPath, OfficialFileBase officialFileBase)
        {
            //Try to find file at file repo 
            if (!this.OfficialProviRdeFilesRepo.ContainsKey(officialFileBase.Id))
            {
                throw new Exception(String.Format("Can not find official file:{0} with id:{1} at current repo. Try to change repo or contact instance author to correct provide info."));
                return;
            }

            var officialRemoteFile = this.OfficialProviRdeFilesRepo[officialFileBase.Id];

            DownloadUtils.DownloadFile(officialRemoteFile.DownloadPath, officialFileBase.LocalPath, officialRemoteFile.Md5);
            Logger.GetLogger().Info(String.Format("Successfully downloaded file:{0} from remote url:{1}.", officialRemoteFile.DownloadPath, rootPath + officialFileBase.LocalPath));

        }

        public void GetCustomFile(String rootPath, CustomFileBase customFileBase)
        {
            DownloadUtils.DownloadFile(customFileBase.DownloadPath, customFileBase.LocalPath, customFileBase.Md5);
            Logger.GetLogger().Info(String.Format("Successfully downloaded file:{0} from remote url:{1}.", customFileBase.DownloadPath, rootPath + customFileBase.LocalPath));
        }

        public void GetEntirePackage(DirectoryInfo packRootFolder, EntirePackageFile entirePackageFile)
        {
            DownloadUtils.DownloadZippedFile(entirePackageFile.DownloadPath, packRootFolder.FullName, entirePackageFile.Md5);
            Logger.GetLogger().Info(String.Format("Successfully downloaded file:{0} then extracted to {1}.", entirePackageFile.DownloadPath, packRootFolder.FullName));
        }
    }
}
