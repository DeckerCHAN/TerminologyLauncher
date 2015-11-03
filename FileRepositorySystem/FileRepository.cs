using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using TerminologyLauncher.Configs;
using TerminologyLauncher.Entities.FileRepository;
using TerminologyLauncher.Entities.InstanceManagement.FileSystem;
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
        public FileRepository(String configPath)
        {
            Logger.GetLogger().Info("Initializing file repo...");
            this.Config = new Config(new FileInfo(configPath));
            this.RepoUrl = this.Config.GetConfigString("fileRepositoryUrl");
            this.OfficialProviRdeFilesRepo = new Dictionary<string, RepositoryFileEntity>();
            Logger.GetLogger().Info("Initialized file repo!");

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

        public RepositoryFileEntity GetOfficialFile(String id)
        {
            if (!this.OfficialProviRdeFilesRepo.ContainsKey(id))
            {
                throw new Exception(String.Format("Can not find official file with id:{0} at current repo. Try to change repo or contact instance author to correct provide info.", id));
            }
            return this.OfficialProviRdeFilesRepo[id];
        }

    }
}
