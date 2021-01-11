
using System.Net.Http.Headers;

namespace TheProcessE
{
    public interface IAuthModel
    {
        AuthenticationHeaderValue Authorization { get; }

        IAuthModel Create();
    }
}
