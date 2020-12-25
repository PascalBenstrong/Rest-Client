using System;

namespace TheProcessE.RestApiClient
{
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false, Inherited = true)]
    public class QUERY : Attribute
    {
        public string Name { get; internal set; } = null;

        public QUERY(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("QUERY is missing name!");

            Name = name;
        }
    }
}
