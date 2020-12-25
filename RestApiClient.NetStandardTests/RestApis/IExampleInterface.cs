using TheProcessE.RestApiClient;

namespace RestApiClient.NetStandardTests.RestApis
{
    [URL("https://jsonplaceholder.typicode.com")]
    public interface IExampleInterface
    {
        [GET("posts")]
        RequestBuilder GetPosts([PARAM] int? id = null);

        [GET("posts")]
        RequestBuilder GetPostById([QUERY("postId")] int id);

        [POST("posts")]
        RequestBuilder PostPosts<TBody>([BODY] TBody post);

    }
}
