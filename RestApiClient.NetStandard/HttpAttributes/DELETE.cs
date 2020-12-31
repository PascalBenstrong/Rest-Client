
using System.Net.Http;

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
