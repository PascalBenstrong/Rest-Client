using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Threading.Tasks;

namespace TheProcessE.RestApiClient
{
    public class ServiceMethodInfo
    {
        private static ConcurrentDictionary<string, ServiceMethodInfo> cache = new ConcurrentDictionary<string, ServiceMethodInfo>();
        private object[] arguments = Array.Empty<object>();
        private ParameterInfo[] parameters = Array.Empty<ParameterInfo>();
        private readonly Uri uri;
        private readonly HttpMethod HttpMethod;

        private ServiceMethodInfo(IEnumerable<Attribute> classAttributes, IEnumerable<Attribute> methodAttributes)
        {
            var url = @"https://";
            foreach (var attr in classAttributes)
            {
                
                if (attr is URL)
                {
                    var u = attr as URL;
                    url = u.Path;
                    continue;
                }
            }
            
            foreach (var attr in methodAttributes)
            {
                if (attr is HttpMethodAttribute)
                {
                    var u = attr as HttpMethodAttribute;
                    url += @"/"+ u.Path;

                    if (HttpMethod != null)
                        throw new NotSupportedException("Service Methods can only have one Http Method Type!");
                    HttpMethod = u.HttpMethod;
                    continue;
                }
            }

            uri = new Uri(url);

        }

        public static ServiceMethodInfo CreateOrAdd<Parent>(MethodInfo method, object[] arguments ) where Parent : class
        {
            var methodInfo = cache.GetOrAdd(method.Name, CreateNewMethodInfo<Parent>(method));
            methodInfo.SetArguments(arguments);
            return methodInfo;
        }

        private ServiceMethodInfo SetArguments(object[] arguments)
        {
            this.arguments = arguments;
            return this;
        }

        private static ServiceMethodInfo CreateNewMethodInfo<Parent>(MethodInfo method)
        {
            var methodInfo = new ServiceMethodInfo(typeof(Parent).GetCustomAttributes(), method.GetCustomAttributes());
            methodInfo.parameters = method.GetParameters();
            return methodInfo;
        }

        public async Task<string> Serialize<T>(T data)
        {
            return await Task.Factory.StartNew(() => JsonConvert.SerializeObject(data));
        }
        
        public async Task<T> Deserialize<T>(string data)
        {
            return await Task.Factory.StartNew(() => JsonConvert.DeserializeObject<T>(data));
        }

        public async Task<Response> Execute()
        {
            // create a string content from object param
            Task<string> bodyTask = null;

            List<KeyValuePair<string, string>> headersParams = new List<KeyValuePair<string, string>>();

            for(int i= 0; i < parameters.Length; i++)
            {
                if(i < arguments.Length)
                {
                    var attributes = parameters[i].GetCustomAttributes();
                    foreach (var attr in attributes)
                    {
                        if (attr is BODY)
                        {
                            bodyTask = Serialize(arguments[i]);
                            continue;
                        }

                        if (attr is Header)
                        {
                            var h = attr as Header;
                            headersParams.Add(new KeyValuePair<string, string>(h.Key, arguments[i] as string));
                            continue;
                        }

                        if (attr is Headers)
                        {
                            var hs = attr as Headers;
                            headersParams.AddRange(hs.headers);
                        }

                    }
                }
            }
            // get the class params and build them

            var body = bodyTask != null ? await bodyTask : "";
            var content = new StringContent(body);

            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            // add the headers to the string content

            foreach (var kp in headersParams)
            {
                content.Headers.Add(kp.Key, kp.Value);
            }

            // create http client and make a call

            HttpResponseMessage responseM = default;

            Response response;

            using (var client = new HttpClient())
            {
                using(var requestM = new HttpRequestMessage())
                {
                    try
                    {
                        requestM.Content = content;
                        requestM.RequestUri = uri;
                        requestM.Method = HttpMethod;

                        responseM = await client.SendAsync(requestM);

                        if (responseM != null && responseM.IsSuccessStatusCode)
                        {
                            var b = await responseM.Content.ReadAsStringAsync();
                            //var result = await Deserialize<object>(b);
                            response = new Response(b, responseM);
                        }
                    }
                    catch
                    {
                        
                    }finally
                    {
                        response = new Response(default, responseM ?? default);
                    }
                }
            }

            return response;
        }

    }
}
