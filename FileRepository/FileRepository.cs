using System;
using System.Collections.Generic;
using System.IO;
using TerminologyLauncher.Configs;
using TerminologyLauncher.Entities.FileRepository;
using TerminologyLauncher.Entities.InstanceManagement;
using TerminologyLauncher.Entities.InstanceManagement.Remote;
using TerminologyLauncher.Entities.SerializeUtils;
using TerminologyLauncher.Logging;
using TerminologyLauncher.Utils;

namespace TerminologyLauncher.FileRepository
{
    public class FileRepository
    {
        public String RepoUrl { get; set; }
        public Config Config { get; set; }
        public Dictionary<String, OfficialFileEntity> OfficialProviRdeFilesRepo { get; set; }
        public FileRepository()
        {
            Logger.GetLogger().Info("Initializing file repo...");
            this.Config = new Config(new FileInfo("Configs/FileRepositoryConfig.json"));
            this.RepoUrl = this.Config.GetConfig("FileRepositoryUrl");
            this.OfficialProviRdeFilesRepo = new Dictionary<string, OfficialFileEntity>();
            Logger.GetLogger().Info("Initialized file repo!");

        }

        public void LoadRepo()
        {
            DownloadUtils.DownloadFile(this.RepoUrl, this.Config.GetConfig("RepoFilePath"));
            var repo = JsonConverter.Parse<FileRepositoryEntity>(File.ReadAllText(this.Config.GetConfig("RepoFilePath")));
            foreach (var officialProvideFile in repo.Files)
            {
                this.OfficialProviRdeFilesRepo.Add(officialProvideFile.ProvideId, officialProvideFile);
            }
        }

        public void GetOfficialFile(String rootPath, OfficialFileEntity officialFileBase)
        {
            //Try to find file at file repo 
            if (!this.OfficialProviRdeFilesRepo.ContainsKey(officialFileBase.ProvideId))
            {
                throw new Exception(String.Format("Can not find official file:{0} with id:{1} at current repo. Try to change repo or contact instance author to correct provide info."));
                return;
            }

            var officialRemoteFile = this.OfficialProviRdeFilesRepo[officialFileBase.ProvideId];

            DownloadUtils.DownloadFile(officialRemoteFile.DownloadLink, officialFileBase.LocalPath, officialRemoteFile.Md5);
            Logger.GetLogger().Info(String.Format("Successfully downloaded file:{0} from remote url:{1}.", officialRemoteFile.DownloadLink, rootPath + officialFileBase.LocalPath));

        }

        public void GetCustomFile(String rootPath, CustomFileEntity customFileBase)
        {
            DownloadUtils.DownloadFile(customFileBase.DownloadLink, customFileBase.LocalPath, customFileBase.Md5);
            Logger.GetLogger().Info(String.Format("Successfully downloaded file:{0} from remote url:{1}.", customFileBase.DownloadLink, rootPath + customFileBase.LocalPath));
        }

        public void GetEntirePackage(DirectoryInfo packRootFolder, EntirePackageFileEntity entirePackageFile)
        {
            DownloadUtils.DownloadZippedFile(entirePackageFile.DownloadPath, packRootFolder.FullName, entirePackageFile.Md5);
            Logger.GetLogger().Info(String.Format("Successfully downloaded file:{0} then extracted to {1}.", entirePackageFile.DownloadPath, packRootFolder.FullName));
        }
    }
}
