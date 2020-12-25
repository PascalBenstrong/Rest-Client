using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheProcessE.RestApiClient
{
    [AttributeUsage(AttributeTargets.Method)]
    public class HEADERS : Attribute
    {
        public List<KeyValuePair<string, string>> headers { get; }

        public HEADERS(List<KeyValuePair<string, string>> headers)
        {
            this.headers = headers;
        }
    }
}
