using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

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
