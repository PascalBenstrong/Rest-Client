using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using TheProcessE.HttpAttributes;
using TheProcessE.RestApiClient;

namespace RestApiClient.NetStandardTests.RestApis
{
    [URL(@"https://parteeapp.herokuapp.com/v1")]
    [Header(typeof(AuthModel))]
    public interface IParteeService
    {
        [GET]
        Task<Response> V1();

        [POST("login")]
        Task<Response> Login<T>([BODY] T model);

        [POST("login")]
        Task<Response> Login();

        [PATCH("listing")]
        Task<Response> PublishListing<T>([Param] Guid listingId, [BODY] T data);

        [POST("profileImage")]
        Task<Response> UploadProfile([BODY("image")] Stream imageStream);

        [GET("image")]
        Task<Response> GetImage([Param] string fileName);
    }
}
