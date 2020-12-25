
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Reflection;

namespace TheProcessE.RestApiClient
{
    public partial class ServiceMethodInfo
    {
        internal static readonly ConcurrentDictionary<string, ServiceMethodInfo> cache = new ConcurrentDictionary<string, ServiceMethodInfo>();
        private object[] arguments = Array.Empty<object>();
        private ParameterInfo[] parameters = Array.Empty<ParameterInfo>();
        private string _url;
        private readonly HttpMethod HttpMethod;
        private HttpClient client { get; set; }
        private readonly IDictionary<string, string> headersParams = new Dictionary<string, string>();

        private ServiceMethodInfo(IEnumerable<Attribute> classAttributes, IEnumerable<Attribute> methodAttributes, HttpClient client)
        {
            this.client = client;
            var hasBaseAddress = false;

            if(this.client.BaseAddress != default && !string.IsNullOrWhiteSpace(this.client.BaseAddress.AbsoluteUri))
            {
                hasBaseAddress = true;
                _url = this.client.BaseAddress.AbsoluteUri;
            }
            else
            {
                _url = "https://";
            }
            foreach (var attr in classAttributes)
            {
                if (attr is URL u)
                {
                    if (hasBaseAddress)
                    {
                        _url += $"{u.Path}";
                    }
                    else
                    {
                        _url = $@"{u.Path}";
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
                    _url += $"/{u.Path}";

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

        public static ServiceMethodInfo GetServiceMethodInfo(MethodInfo method)
        {
            var declaring = method.DeclaringType;
            var assembly = declaring.AssemblyQualifiedName;
            return cache.GetValueOrDefault(method.Name);
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
                            _url = ParseParams(_url, ref param, i, ref arguments);
                        }
                    }
                }
            }

            if(bodyContent == default)
            {
                bodyContent = new StringContent("");
            }
            // add the headers to the string content

            client.DefaultRequestHeaders.Clear();
            foreach (var kp in headersParams)
            {
                client.DefaultRequestHeaders.Add(kp.Key, kp.Value);
            }

            var uri = new Uri(_url);
            return new RequestBuilder(HttpMethod, bodyContent, uri, client);

        }

    }
}
