using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Text.Json;
using System.Threading;
using System.Collections.Generic;
using System.Net.Http.Headers;

namespace TheProcessE.RestApiClient
{
    public class RequestBuilder
    {
        public Exception Exception { get; private set; }
        public HttpResponseMessage HttpResponseMessage { get; private set; }

        public bool IsConnectionError { get; private set; }

        public bool IsSuccess => !IsConnectionError && HttpResponseMessage.IsSuccessStatusCode;

        public HttpStatusCode StatusCode => HttpResponseMessage.StatusCode;

        private readonly HttpMethod HttpMethod;
        private readonly HttpContent HttpContent;
        private readonly Uri Uri;
        private readonly HttpClient Client;
        private readonly HttpRequestHeaders Headers;

        internal RequestBuilder(HttpMethod httpMethod, HttpContent httpContent, Uri uri, HttpClient client, IDictionary<string,string> headers)
        {
            HttpMethod = httpMethod;
            HttpContent = httpContent;
            Uri = uri;
            Client = client;
            using var httpMessage = new HttpRequestMessage();
            Headers = httpMessage.Headers;

            foreach(var header in headers)
            {
                Headers.Add(header.Key, header.Value);
            }
        }

        public void AddHeader(string key, string value)
        {
            Headers.Add(key, value);
        }
        
        public void AddHeader(string key, IEnumerable<string> values)
        {
            Headers.Add(key, values);
        }

        public async ValueTask<string> GetResponseAsStringAsync(CancellationToken cancellationToken = default)
        {
            var response = await SendAsync(cancellationToken);

            if (IsConnectionError)
                return default;

            var result = await response.Content.ReadAsStringAsync();
            return result;
        }

        public async ValueTask<TResult> GetResponseAsync<TResult>(JsonSerializerOptions? options = default, CancellationToken cancellationToken = default)
        {
            var response = await SendAsync(cancellationToken);

            if (IsConnectionError)
                return default;

            if (options == default)
            {
                options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };
            }

            var contentStream = await response.Content.ReadAsStreamAsync();
            var result = await JsonSerializer.DeserializeAsync<TResult>(contentStream, options, cancellationToken);
            return result;
        }

        public async ValueTask<TResult> GetResponseSuppressExceptionAsync<TResult>(JsonSerializerOptions? options = default, CancellationToken cancellationToken = default)
        {

            try
            {
                var result = await GetResponseAsync<TResult>(options,cancellationToken);
                return result;
            }
            catch(Exception e)
            {
                Exception = e;
                return default;
            }
        }

        public async ValueTask<HttpResponseMessage> SendAsync(CancellationToken cancellationToken = default)
        {
            using var requestM = new HttpRequestMessage();

            if (HttpMethod != HttpMethod.Get)
                requestM.Content = HttpContent;
            requestM.RequestUri = Uri;
            requestM.Method = HttpMethod;
            // add the headers to the string content

            Client.DefaultRequestHeaders.Clear();
            foreach (var kp in Headers)
            {
                Client.DefaultRequestHeaders.Add(kp.Key, kp.Value);
            }

            if (cancellationToken != default)

                HttpResponseMessage = await Client.SendAsync(requestM, cancellationToken);
            else

                HttpResponseMessage = await Client.SendAsync(requestM);

            return HttpResponseMessage;
        }

        public async ValueTask<HttpResponseMessage> SendSuppressExceptionAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                return await SendAsync(cancellationToken);
            }catch(Exception e)
            {
                IsConnectionError = true;
                Exception = e;
                return default;
            }

        }

    }
}
