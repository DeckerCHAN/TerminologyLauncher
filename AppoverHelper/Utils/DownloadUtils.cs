using System;
using System.IO;
using System.Net.Cache;
using System.Threading.Tasks;
using AppoverHelper.Utils.Net;

namespace AppoverHelper.Utils
{
    public static class DownloadUtils
    {
       


        public static string GetWebContent(string url)
        {
            using (var client = new TerminologyWebClient())
            {
                client.CachePolicy = new RequestCachePolicy(RequestCacheLevel.NoCacheNoStore);
                client.Encoding = EncodeUtils.NoneBomUtf8;
                return client.DownloadString(url);
            }
        }

    }
}