using System;

namespace TheProcessE.RestApiClient
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface)]
    public sealed class URL : Attribute
    {
        public string Path { get; }

        public URL(string path)
        {
            Path = path;
        }
    }
}
