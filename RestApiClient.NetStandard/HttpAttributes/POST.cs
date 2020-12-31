
using System.Net.Http;

namespace TheProcessE.RestApiClient
{
    public class POST : HttpMethodAttribute
    {
        public POST()
        {
            HttpMethod = HttpMethod.Post;
        }

        public POST(string path) : this()
        {
            Path = path;
        }
    }
}
