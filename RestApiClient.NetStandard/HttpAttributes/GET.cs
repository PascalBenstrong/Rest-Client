using System;
using System.Net.Http;

namespace TheProcessE.RestApiClient
{
    public class GET : HttpMethodAttribute
    {
        public GET()
        {
            HttpMethod = HttpMethod.Get;
        }

        public GET(string path) : this()
        {
            Path = path;
        }
    }
}
