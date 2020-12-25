using System;
using System.Collections.Generic;
using System.Text;

namespace TheProcessE.RestApiClient
{
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false,Inherited = true)]
    public class PARAM : Attribute
    {
        public string Name { get; internal set; } = null;
        internal readonly bool IsNamed;
        public PARAM()
        {}

        public PARAM(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Named PARAM attribute may not be null or empty!");

            Name = name;
            IsNamed = true;
        }
    }
}
