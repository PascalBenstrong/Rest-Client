
using System.Net.Http;

namespace TheProcessE.RestApiClient
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
