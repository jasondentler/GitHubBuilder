using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace GitHubBuilder.Console
{
    public static class JsonHelpers
    {

        public static JObject GetJson(string urlTemplate, params object[] parms)
        {
            return GetJson(string.Format(urlTemplate, parms));
        }

        public static JObject GetJson(string url)
        {
            var wc = new WebClient();
            var data = wc.DownloadString(url);
            return JObject.Parse(data);
        }

    }
}
