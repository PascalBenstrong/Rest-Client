using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace TheProcessE.RestApiClient
{
    public class Response
    {
        public string ResponseBody { get; }
        public HttpResponseMessage HttpResponseMessage { get; }

        public bool IsConnectionError => HttpResponseMessage == null || HttpResponseMessage == default;

        internal Response(string responseBody, HttpResponseMessage httpResponseMessage)
        {
            ResponseBody = responseBody;
            HttpResponseMessage = httpResponseMessage;
        }

        public TResult GetResponse<TResult>()
        {
            return Newtonsoft.Json.JsonConvert.DeserializeObject<TResult>(ResponseBody ?? "");
        }
    }
}
