
namespace TheProcessE
{
    public interface IAuthModel
    {
        string Scheme { get; }
        string Token { get; }

        IAuthModel Create();
    }
}
