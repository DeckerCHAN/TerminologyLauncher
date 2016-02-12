using System;
using System.Net;

namespace TerminologyLauncher.Utils.Net
{
    public class TerminologyWebClient : WebClient
    {
        protected override WebRequest GetWebRequest(Uri address)
        {
            var request = base.GetWebRequest(address);
            if (request is HttpWebRequest)
            {
                ((HttpWebRequest)request).KeepAlive = false;
                ((HttpWebRequest)request).ProtocolVersion = HttpVersion.Version11;
            }

            return request;
        }
    }
}
