using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheProcessE.RestApiClient
{
    [AttributeUsage(AttributeTargets.Method)]
    public class Headers : Attribute
    {
        public List<KeyValuePair<string, string>> headers { get; }

        public Headers(List<KeyValuePair<string, string>> headers)
        {
            this.headers = headers;
        }
    }
}
