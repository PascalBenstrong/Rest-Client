using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace TheProcessE.RestApiClient
{
    public sealed class DELETE : HttpMethodAttribute
    {
        public DELETE()
        {
            HttpMethod = HttpMethod.Delete;
        }

        public DELETE(string path) : this()
        {
            Path = path;
        }
    }
}
