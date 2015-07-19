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
using TerminologyLauncher.Utils.ProgressService;

namespace TerminologyLauncher.FileRepository
{
    public class FileRepository
    {
        public String RepoUrl { get; set; }
        public Config Config { get; set; }
        private Dictionary<String, OfficialFileEntity> OfficialProviRdeFilesRepo { get; set; }
        public FileRepository(String configPath)
        {
            Logger.GetLogger().Info("Initializing file repo...");
            this.Config = new Config(new FileInfo(configPath));
            this.RepoUrl = this.Config.GetConfig("fileRepositoryUrl");
            this.OfficialProviRdeFilesRepo = new Dictionary<string, OfficialFileEntity>();
            Logger.GetLogger().Info("Initialized file repo!");

        }

        public void LoadRepo()
        {
            DownloadUtils.DownloadFile(this.RepoUrl, this.Config.GetConfig("repoFilePath"));
            var repo = JsonConverter.Parse<FileRepositoryEntity>(File.ReadAllText(this.Config.GetConfig("repoFilePath")));
            foreach (var officialProvideFile in repo.Files)
            {
                this.OfficialProviRdeFilesRepo.Add(officialProvideFile.ProvideId, officialProvideFile);
            }
        }

        public void ReceiveOfficialFile(LeafNodeProgress progress, DirectoryInfo packRootFolder, OfficialFileEntity officialFile)
        {


            //Try to find file at file repo 
            if (!this.OfficialProviRdeFilesRepo.ContainsKey(officialFile.ProvideId))
            {
                throw new Exception(String.Format("Can not find official file:{0} with id:{1} at current repo. Try to change repo or contact instance author to correct provide info."));
            }

            var officialRemoteFile = this.OfficialProviRdeFilesRepo[officialFile.ProvideId];

            var downloadLink = officialRemoteFile.DownloadLink;
            var downloadTargetPositon = Path.Combine(packRootFolder.FullName, officialRemoteFile.LocalPath);

            ProgressSupportedDownloadUtils.DownloadFile(progress, downloadLink, downloadTargetPositon, officialRemoteFile.Md5);
            Logger.GetLogger().Info(String.Format("Successfully downloaded file:{0} from remote url:{1}.", downloadTargetPositon, downloadLink));

        }

        public void ReceiveCustomFile(LeafNodeProgress progress, DirectoryInfo packRootFolder, CustomFileEntity customFile)
        {
            var downloadLink = customFile.DownloadLink;
            var downloadTargetPositon = Path.Combine(packRootFolder.FullName, customFile.LocalPath);
            ProgressSupportedDownloadUtils.DownloadFile(progress, downloadLink, downloadTargetPositon, customFile.Md5);
            Logger.GetLogger().Info(String.Format("Successfully downloaded file:{0} from remote url:{1}.", downloadTargetPositon, downloadLink));
        }

        public void ReceiveEntirePackage(InternalNodeProgress progress, DirectoryInfo packRootFolder, EntirePackageFileEntity entirePackageFile)
        {
            var downloadLink = entirePackageFile.DownloadLink;
            var downloadTargetPositon = Path.Combine(packRootFolder.FullName, entirePackageFile.LocalPath);
            DownloadUtils.DownloadZippedFile(downloadLink, downloadTargetPositon, entirePackageFile.Md5);
            Logger.GetLogger().Info(String.Format("Successfully downloaded file:{0} then extracted to {1}.", downloadLink, downloadTargetPositon));
        }
    }
}
