using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ZeroCDN_Client
{
    public class WebClientEx : WebClient
    {
        private CookieContainer _cookieContainer = new CookieContainer();

        protected override WebRequest GetWebRequest(Uri address)
        {
            WebRequest request = base.GetWebRequest(address);
            if (request is HttpWebRequest)
            {
                (request as HttpWebRequest).CookieContainer = _cookieContainer;
            }

            return request;
        }
    }
}
