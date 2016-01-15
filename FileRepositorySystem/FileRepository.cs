using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using TerminologyLauncher.Configs;
using TerminologyLauncher.Entities.FileRepository;
using TerminologyLauncher.Logging;
using TerminologyLauncher.Utils;
using TerminologyLauncher.Utils.ProgressService;
using TerminologyLauncher.Utils.SerializeUtils;

namespace TerminologyLauncher.FileRepositorySystem
{
    public class FileRepository
    {
        public String RepoUrl { get; set; }
        public Config Config { get; set; }
        private Dictionary<String, RepositoryFileEntity> OfficialProviRdeFilesRepo { get; set; }
        private DirectoryInfo CacheDirectoryInfo { get; set; }
        private DirectoryInfo CacheRootDirectoryInfo { get; set; }
        public FileRepository(String configPath)
        {
            Logger.GetLogger().Info("Initializing file repo...");
            this.Config = new Config(new FileInfo(configPath));
            this.RepoUrl = this.Config.GetConfigString("fileRepositoryUrl");
            this.OfficialProviRdeFilesRepo = new Dictionary<string, RepositoryFileEntity>();
            Logger.GetLogger().Info("Initialized file repo!");
            this.CacheRootDirectoryInfo = new DirectoryInfo(Config.GetConfigString("repoCacheFolder"));
            if (!this.CacheRootDirectoryInfo.Exists)
            {
                this.CacheRootDirectoryInfo.Create();
            }
            Logger.GetLogger().Info("Finished create repo repository folder.");
            //Different repo url will not share same cache folder.
            this.CacheDirectoryInfo = new DirectoryInfo(Path.Combine(this.CacheRootDirectoryInfo.FullName, EncodeUtils.CalculateStringMd5(this.Config.GetConfigString("fileRepositoryUrl"))));
            if (!this.CacheDirectoryInfo.Exists)
            {
                this.CacheDirectoryInfo.Create();
            }
            Logger.GetLogger().InfoFormat("Created cache directory {0}", this.CacheDirectoryInfo.Name);

            Logger.GetLogger().Info(String.Format("Start to fetch repo from url {0}", RepoUrl));
            try
            {
                var progress = new LeafNodeProgress("Fetch repo");
                DownloadUtils.DownloadFile(progress, this.RepoUrl, this.Config.GetConfigString("repoFilePath"));
                var repo =
                    JsonConverter.Parse<FileRepositoryEntity>(File.ReadAllText(this.Config.GetConfigString("repoFilePath")));
                foreach (var officialProvideFile in repo.Files)
                {
                    this.OfficialProviRdeFilesRepo.Add(officialProvideFile.ProvideId, officialProvideFile);
                }

            }
            catch (WebException)
            {
                Logger.GetLogger().Error("Unable to receive repo right now. Trying to using local repo list.");
                if (!File.Exists(this.Config.GetConfigString("repoFilePath")))
                {
                    Logger.GetLogger().Error("No local repo list available.");
                    return;
                }
                var repo =
                   JsonConverter.Parse<FileRepositoryEntity>(File.ReadAllText(this.Config.GetConfigString("repoFilePath")));
                foreach (var officialProvideFile in repo.Files)
                {
                    this.OfficialProviRdeFilesRepo.Add(officialProvideFile.ProvideId, officialProvideFile);
                }
                Logger.GetLogger().Info("Used local repo list.");
            }
            catch (Exception)
            {
                throw;
            }

        }

        public FileInfo GetOfficialFile(LeafNodeProgress progress, String id)
        {
            if (!this.OfficialProviRdeFilesRepo.ContainsKey(id))
            {
                throw new Exception(String.Format("Can not find official file with id:{0} at current repo. Try to change repo or contact instance author to correct provide info.", id));
            }
            var repositoryFile = this.OfficialProviRdeFilesRepo[id];
            var cacheFile = new FileInfo(Path.Combine(this.CacheDirectoryInfo.FullName, repositoryFile.ProvideId));
            if (cacheFile.Exists && (EncodeUtils.CalculateFileMd5(cacheFile.FullName).Equals(repositoryFile.Md5)))
            {
                progress.Percent = 100D;
                Logger.GetLogger().InfoFormat("File {0} exists and md5 check successful. Using cache file.", repositoryFile.Name);
            }
            else
            {
                Logger.GetLogger().InfoFormat("File {0} not exists or md5 check fault. Download new file.", repositoryFile.Name);

                cacheFile.Delete();
                DownloadUtils.DownloadFile(progress, repositoryFile.DownloadPath, cacheFile.FullName);
                Logger.GetLogger().InfoFormat("Successful download file {0} from url {1}", repositoryFile.Name, repositoryFile.DownloadPath);
            }
            return cacheFile;

        }

    }
}
