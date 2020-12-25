using TheProcessE.RestApiClient;

namespace RestApiClient.NetStandardTests.RestApis
{
    [URL("http://localhost:2999")]
    public interface IExampleInterface
    {
        [GET("posts")]
        RequestBuilder Posts([PARAM] int? id = null);

        [POST("posts")]
        RequestBuilder PostPosts<TBody>([BODY] TBody post);

        [GET("todos")]
        RequestBuilder Todos();

        [GET("users")]
        RequestBuilder Users();
    }
}
