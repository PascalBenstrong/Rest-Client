
using System.Net.Http;

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
