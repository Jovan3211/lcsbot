using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lcsbot.Services
{
    public static class HTTPHandler
    {
        public static IRestResponse GET(string url)
        {
            var client = new RestClient(url);
            IRestResponse response = client.Execute(new RestRequest());

            return response;
        }
    }
}
