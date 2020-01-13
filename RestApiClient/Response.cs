﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace TheProcessE.RestApiClient
{
    public class Response
    {
        public string Result { get; }
        public HttpResponseMessage HttpResponseMessage { get; }

        public bool IsConnectionError => HttpResponseMessage == null || HttpResponseMessage == default;

        internal Response(string result, HttpResponseMessage httpResponseMessage)
        {
            Result = result;
            HttpResponseMessage = httpResponseMessage;
        }

        public TResult GetResult<TResult>()
        {
            return Newtonsoft.Json.JsonConvert.DeserializeObject<TResult>(Result ?? "");
        }
    }
}
