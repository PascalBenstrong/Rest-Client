using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace TheProcessE.RestApiClient
{
    public sealed class PUT : HttpMethodAttribute
    {
        public PUT()
        {
            HttpMethod = HttpMethod.Put;
        }

        public PUT(string path) : this()
        {
            Path = path;
        }
    }
}
