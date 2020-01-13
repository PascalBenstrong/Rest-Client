using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace TheProcessE.RestApiClient
{
    [AttributeUsage(AttributeTargets.Method)]
    public abstract class HttpMethodAttribute : Attribute
    {
        public string Path { get; internal set; } = "";
        public HttpMethod HttpMethod { get; internal set; }
    }
}
