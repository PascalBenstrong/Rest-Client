using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;

namespace TheProcessE.RestApiClient
{
    public class Response : IDisposable
    {
        public string ResponseBody => Encoding.UTF8.GetString(ResponseBytes);
        public byte[] ResponseBytes { get; private set; }
        public Stream ResponseStream => new MemoryStream(ResponseBytes);
        public HttpResponseMessage HttpResponseMessage { get; }

        public bool IsConnectionError => HttpResponseMessage == null || HttpResponseMessage == default;

        public bool IsSuccess => !IsConnectionError && HttpResponseMessage.IsSuccessStatusCode;

        public HttpStatusCode StatusCode => HttpResponseMessage.StatusCode;

        internal Response(byte[] responseBytes, HttpResponseMessage httpResponseMessage)
        {
            ResponseBytes = responseBytes;
            HttpResponseMessage = httpResponseMessage;
        }

        public TResult GetResponse<TResult>()
        {
            return Newtonsoft.Json.JsonConvert.DeserializeObject<TResult>(ResponseBody ?? "");
        }

        public void Dispose()
        {
            ResponseBytes = Array.Empty<byte>();
            if(HttpResponseMessage != null)
            {
                HttpResponseMessage.Dispose();
            }
        }
    }
}
