﻿using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Text.Json;

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

        internal RequestBuilder(HttpMethod httpMethod, HttpContent httpContent, Uri uri, HttpClient client)
        {
            HttpMethod = httpMethod;
            HttpContent = httpContent;
            Uri = uri;
            Client = client;
        }

        public async ValueTask<string> GetResponseAsStringAsync()
        {
            var response = await SendAsync();

            if (IsConnectionError)
                return default;

            var result = await response.Content.ReadAsStringAsync();
            return result;
        }

        public async ValueTask<TResult> GetResponseAsync<TResult>()
        {
            var response = await SendAsync();

            if (IsConnectionError)
                return default;

            var contentStream = await response.Content.ReadAsStreamAsync();
            var result = await JsonSerializer.DeserializeAsync<TResult>(contentStream);
            return result;
        }

        public async ValueTask<TResult> GetResponseSuppressExceptionAsync<TResult>()
        {
            var response = await SendSuppressExceptionAsync();

            if (IsConnectionError)
                return default;

            try
            {
                var contentStream = await response.Content.ReadAsStreamAsync();
                var result = await JsonSerializer.DeserializeAsync<TResult>(contentStream);
                return result;
            }
            catch(Exception e)
            {
                Exception = e;
                return default;
            }
        }
        public async ValueTask<HttpResponseMessage> SendAsync()
        {
            using var requestM = new HttpRequestMessage();

            if (HttpMethod != HttpMethod.Get)
                requestM.Content = HttpContent;
            requestM.RequestUri = Uri;
            requestM.Method = HttpMethod;

            HttpResponseMessage = await Client.SendAsync(requestM);
            return HttpResponseMessage;
        }

        public async ValueTask<HttpResponseMessage> SendSuppressExceptionAsync()
        {
            try
            {
                return await SendAsync();
            }catch(Exception e)
            {
                IsConnectionError = true;
                Exception = e;
                return default;
            }

        }

    }
}
