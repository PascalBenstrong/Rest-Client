using System;

namespace TheProcessE.RestApiClient
{

    [AttributeUsage(AttributeTargets.Parameter)]
    public class BODY : Attribute
    {
        internal string Name { get; }

        public BODY() : this(null)
        {
        }
        public BODY(string name)
        {
            Name = name;
        }
    }
}
