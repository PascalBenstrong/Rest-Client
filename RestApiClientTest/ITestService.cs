using TheProcessE.RestApiClient;
using System.Net.Http;
using System.Threading.Tasks;

namespace RestApiClientTest
{
    [URL("https://parteeapp.herokuapp.com/v1")]
    public interface ITestService
    {
        [POST("login")]
        Task<Response> login<Data>([BODY]Data data);

        [GET]
        Task<Response> index();
    }
}
