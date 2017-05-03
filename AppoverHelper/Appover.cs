using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppoverHelper.Structure;

namespace AppoverHelper
{
    public class Appover
    {
        public string Account { get; private set; }
        public string Slag { get; private set; }
        private const string urlPerffix = "https://ci.appveyor.com/api/projects";


        private History GetBuildHistory(ushort recordsNumber, ushort startBuildId)
        {
            var url = string.Format($"{urlPerffix}/projects/{this.Account}/{this.Slag}/history?recordsNumber={recordsNumber}&startBuildId={startBuildId}");
            var content = Utils.DownloadUtils.GetWebContent(url);
            return Utils.JsonConverter.Parse<History>(content);
        }

        public Appover(string account, string slag)
        {
            this.Account = account;
            this.Slag = slag;
        }
    }
}