using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

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
