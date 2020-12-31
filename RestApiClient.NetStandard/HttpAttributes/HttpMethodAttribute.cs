using System;
using System.Net.Http;

namespace TheProcessE.RestApiClient
{
    [AttributeUsage(AttributeTargets.Method)]
    public abstract class HttpMethodAttribute : Attribute
    {
        public string Path { get; internal set; } = "";
        public HttpMethod HttpMethod { get; internal set; }
    }
}
