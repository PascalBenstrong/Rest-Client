using System;
using System.Collections.Generic;
using System.Text;
using TheProcessE.RestApiClient;
using System.Net.Http;

namespace TheProcessE.HttpAttributes
{
    public class PATCH : HttpMethodAttribute
    {
        public PATCH()
        {
            HttpMethod = new HttpMethod("PATCH");
        }

        public PATCH(string path) : this()
        {
            Path = path;
        }
    }
}
