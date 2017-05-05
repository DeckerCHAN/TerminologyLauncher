using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using AppoverHelper.Structure;

namespace AppoverHelper
{
    public class Appover
    {
        public string Account { get; private set; }
        public string Slag { get; private set; }

        private HashSet<Build> BuildCache { get; set; }
        private const string ProjectUrlPerffix = "https://ci.appveyor.com/api/projects";
        private const string JobUrlPerffix = "https://ci.appveyor.com/api/buildjobs";
        private const int BuildAmountPreFecht = 10;


        private History GetBuildHistory(int recordsNumber, int startBuildId)
        {
            var url = string.Format(
                $"{ProjectUrlPerffix}/{this.Account}/{this.Slag}/history?recordsNumber={recordsNumber}&startBuildId={startBuildId}");
            var content = Utils.DownloadUtils.GetWebContent(url);
            return Utils.JsonConverter.Parse<History>(content);
        }

        private History GetBuildHistory(int recordsNumber)
        {
            var url = string.Format(
                $"{ProjectUrlPerffix}/{this.Account}/{this.Slag}/history?recordsNumber={recordsNumber}");
            var content = Utils.DownloadUtils.GetWebContent(url);
            return Utils.JsonConverter.Parse<History>(content);
        }

        public ICollection<Artifact> LatestBuildArtificats(string branch)
        {
            var latestBuild = this.LastestSuccessfulBuild(branch);
            if (latestBuild?.Jobs == null || latestBuild.Jobs.Count == 0)
            {
                return null;
            }

            var artifacts = new HashSet<Artifact>();

            foreach (var job in latestBuild.Jobs)
            {
                var artifactUrl = string.Format($"{JobUrlPerffix}/{job.JobId}/artifacts");
                var content = Utils.DownloadUtils.GetWebContent(artifactUrl);
                artifacts.UnionWith(Utils.JsonConverter.Parse<HashSet<Artifact>>(content));
            }
            return artifacts;
        }

        public Build LastestSuccessfulBuild(string branch)
        {
            string latestSuccessfulVersion = null;

            var successfulBuilds = this.BuildCache.Where(x => x.Status.Equals("success") && x.Branch.Equals(branch))
                .ToList();
            if (successfulBuilds.Any())
            {
                latestSuccessfulVersion = successfulBuilds.OrderByDescending(x => x.BuildNumber).First().Version;
            }
            //Other wise
            while (true)
            {
                var fetchedHistory = this.FetchOlderBuilds();

                if (fetchedHistory.Count == 0)
                {
                    return null;
                }

                var statifyingCondition = fetchedHistory
                    .Where(x => x.Status.Equals("success") && x.Branch.Equals(branch))
                    .ToList();


                if (statifyingCondition.Any())
                {
                    latestSuccessfulVersion = statifyingCondition.OrderByDescending(x => x.BuildNumber).First().Version;
                    break;
                }
            }


            var url = string.Format($"{ProjectUrlPerffix}/{this.Account}/{this.Slag}/build/{latestSuccessfulVersion}");

            var content = Utils.DownloadUtils.GetWebContent(url);
            return Utils.JsonConverter.Parse<ProjectWithBuild>(content).Build;
        }

        private ICollection<Build> FetchOlderBuilds()
        {
            if (this.BuildCache.Count > 0)
            {
                var earliestBuildId = this.BuildCache.OrderBy(x => x.BuildNumber).First().BuildId;
                var builds = this.GetBuildHistory(BuildAmountPreFecht, earliestBuildId).Builds;
                this.BuildCache.UnionWith(builds);
                return builds;
            }
            else
            {
                var builds = this.GetBuildHistory(BuildAmountPreFecht).Builds;
                this.BuildCache.UnionWith(builds);
                return builds;
            }
        }

        public Appover(string account, string slag)
        {
            this.Account = account;
            this.Slag = slag;
            this.BuildCache = new HashSet<Build>();
        }
    }
}