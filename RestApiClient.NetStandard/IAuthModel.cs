using System;
using System.Collections.Generic;
using System.Text;

namespace TheProcessE
{
    public interface IAuthModel
    {
        string Scheme { get; }
        string Token { get; }

        IAuthModel Create();
    }
}
