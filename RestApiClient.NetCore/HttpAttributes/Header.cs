using System;
using System.Collections.Generic;

namespace TheProcessE.RestApiClient
{
    [AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Method)]
    public class Header : Attribute
    {
        public string Key { get; }

        public Header(string key)
        {
            Key = key;
        }
    }
}
