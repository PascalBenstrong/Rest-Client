
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net.Http;
using System.Reflection;

namespace TheProcessE.RestApiClient
{
    public partial class ServiceMethodInfo
    {
        internal static readonly ConcurrentDictionary<string, ServiceMethodInfo> cache = new ConcurrentDictionary<string, ServiceMethodInfo>();
        private object[] arguments = Array.Empty<object>();
        private ParameterInfo[] parameters = Array.Empty<ParameterInfo>();
        private readonly string _baseUrl;
        private readonly string _relativeUrl;
        private readonly HttpMethod HttpMethod;
        private readonly HttpClient _client;
        private readonly IDictionary<string, string> headersParams = new Dictionary<string, string>();

        private ServiceMethodInfo(IEnumerable<Attribute> classAttributes, IEnumerable<Attribute> methodAttributes, HttpClient client)
        {
            _client = client;
            var hasBaseAddress = false;

            if(_client.BaseAddress != default && !string.IsNullOrWhiteSpace(_client.BaseAddress.AbsoluteUri))
            {
                hasBaseAddress = true;
                _baseUrl = this._client.BaseAddress.AbsoluteUri;
            }
            else
            {
                _baseUrl = "https://";
            }
            foreach (var attr in classAttributes)
            {
                if (attr is URL u)
                {
                    if (hasBaseAddress)
                    {
                        _baseUrl += $"{u.Path}";
                    }
                    else
                    {
                        _baseUrl = $"{u.Path}";
                    }

                    continue;
                }
                else if (attr is HEADER header)
                {
                    object[] nullArg = null;
                    ParseHeader(headersParams, ref header, 0, ref nullArg);

                    continue;
                }
            }

            foreach (var attr in methodAttributes)
            {
                if (attr is HttpMethodAttribute u)
                {
                    _relativeUrl += _baseUrl[_baseUrl.Length-1] == '/' ? u.Path : $"/{u.Path}";

                    if (HttpMethod != null)
                        throw new NotSupportedException("Service Methods can only have one Http Method Type!");
                    HttpMethod = u.HttpMethod;
                    continue;
                }
                else if (attr is HEADER header)
                {
                    object[] nullArg = null;
                    ParseHeader(headersParams, ref header, 0, ref nullArg);
                    continue;
                }
            }

        }

        public static ServiceMethodInfo CreateOrAdd<Parent>(MethodInfo method, object[] arguments, HttpClient client) where Parent : class
        {
            var methodInfo = cache.GetOrAdd(method.GetSignature(), CreateNewMethodInfo<Parent>(method, client));
            methodInfo.SetArguments(arguments);
            return methodInfo;
        }

        private ServiceMethodInfo SetArguments(object[] arguments)
        {
            this.arguments = arguments;
            return this;
        }

        private static ServiceMethodInfo CreateNewMethodInfo<Parent>(MethodInfo method, HttpClient client)
        {
            var methodInfo = new ServiceMethodInfo(typeof(Parent).GetCustomAttributes(), method.GetCustomAttributes(), client);
            methodInfo.parameters = method.GetParameters();
            return methodInfo;
        }

        public RequestBuilder Execute()
        {
            // clear out any relative url and query
            var relativeUrl = _relativeUrl;
            var _query = string.Empty;
            // create a string content from object param
            HttpContent bodyContent = null;

            for (int i = 0; i < parameters.Length; i++)
            {
                if (i < arguments.Length)
                {
                    var attributes = parameters[i].GetCustomAttributes();
                    foreach (var attr in attributes)
                    {
                        if (attr is BODY body)
                        {
                            if (bodyContent != null)
                                throw new ArgumentException("Only one body can be sent with the request!");

                            HandleBodyContent(ref bodyContent, i, ref body);
                        }
                        else if (attr is HEADER header)
                        {
                            ParseHeader(headersParams, ref header, i, ref arguments, true);
                            continue;
                        }
                        else if (attr is PARAM param)
                        {
                            relativeUrl = ParseParams(_relativeUrl, ref param, i, ref arguments);
                        }else if(attr is QUERY query)
                        {
                            var q = arguments[i]?.ToString();
                            // only add none null or empty query
                            if (!string.IsNullOrWhiteSpace(q))
                            {
                                _query += $"{query.Name}={q}&";
                            }
                        }
                    }
                }

            }

            if(_query.Length > 2)
            {
                _query = _query.Substring(0, _query.Length - 1);
                relativeUrl += $"?{_query}";
            }

            if(bodyContent == default)
            {
                bodyContent = new StringContent("");
            }

            var uri = new Uri(_baseUrl + relativeUrl);
            return new RequestBuilder(HttpMethod, bodyContent, uri, _client, headersParams);

        }

    }
}
