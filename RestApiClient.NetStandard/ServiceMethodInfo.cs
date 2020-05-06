using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace TheProcessE.RestApiClient
{
    public class ServiceMethodInfo
    {
        private static ConcurrentDictionary<string, ServiceMethodInfo> cache = new ConcurrentDictionary<string, ServiceMethodInfo>();
        private object[] arguments = Array.Empty<object>();
        private ParameterInfo[] parameters = Array.Empty<ParameterInfo>();
        private string url { get; set; }
        private Uri uri;
        private readonly HttpMethod HttpMethod;
        private HttpClient client { get; set; }
        private readonly bool recycle;
        private readonly IDictionary<string, string> headersParams = new Dictionary<string, string>();

        private ServiceMethodInfo(IEnumerable<Attribute> classAttributes, IEnumerable<Attribute> methodAttributes, HttpClient client, bool recycle)
        {
            this.client = client;
            this.recycle = recycle;
            url = @"https://";
            foreach (var attr in classAttributes)
            {
                if (attr is URL)
                {
                    var u = attr as URL;
                    url = $@"{u.Path}";
                    continue;
                }
                else if (attr is Header header)
                {
                    if (header.HasAuthModel)
                        headersParams.Add(header.Key, header.Authentication.ToString());
                    else if (!string.IsNullOrWhiteSpace(header.Key) && !string.IsNullOrWhiteSpace(header.Value))
                        headersParams.Add(header.Key, header.Value);

                    continue;
                }
            }

            foreach (var attr in methodAttributes)
            {
                if (attr is HttpMethodAttribute)
                {
                    var u = attr as HttpMethodAttribute;
                    url += $@"/{u.Path}";

                    if (HttpMethod != null)
                        throw new NotSupportedException("Service Methods can only have one Http Method Type!");
                    HttpMethod = u.HttpMethod;
                    continue;
                }
                else if (attr is Header header)
                {
                    if (header.HasAuthModel)
                        headersParams.Add(header.Key, header.Authentication.ToString());
                    else if (!string.IsNullOrWhiteSpace(header.Key) && !string.IsNullOrWhiteSpace(header.Value))
                        headersParams.Add(header.Key, header.Value);
                    continue;
                }
            }

            uri = new Uri(url);

        }

        public static ServiceMethodInfo CreateOrAdd<Parent>(MethodInfo method, object[] arguments, HttpClient client, bool recycle) where Parent : class
        {
            var methodInfo = cache.GetOrAdd(method.Name, CreateNewMethodInfo<Parent>(method, client, recycle));
            methodInfo.SetArguments(arguments);
            return methodInfo;
        }

        private ServiceMethodInfo SetArguments(object[] arguments)
        {
            this.arguments = arguments;
            return this;
        }

        private static ServiceMethodInfo CreateNewMethodInfo<Parent>(MethodInfo method, HttpClient client, bool recycle)
        {
            var methodInfo = new ServiceMethodInfo(typeof(Parent).GetCustomAttributes(), method.GetCustomAttributes(), client, recycle);
            methodInfo.parameters = method.GetParameters();
            return methodInfo;
        }

        private HttpContent Serialize<T>(T data)
        {
            var json = JsonConvert.SerializeObject(data);
            var content = new StringContent(json);

            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            return content;
        }

        private string SerializeObject(object data) => JsonConvert.SerializeObject(data);

        private HttpContent MultiPartForm(Stream stream, string name)
        {
            var form = new MultipartFormDataContent();
            //var memStream = new MemoryStream();
            //stream.CopyTo(memStream);
            var streamContent = new StreamContent(stream);
            var fStream = stream as FileStream;
            var fileName = fStream != null ? fStream.Name : "null";
            streamContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data")
            {
                Name = name,
                FileName = fileName
            };
            form.Add(streamContent, name);

            return form;
        }

        private HttpContent MultiPartForm(List<Stream> streams, string[] names)
        {
            var form = new MultipartFormDataContent();
            int i = 0;
            foreach (var stream in streams)
            {
                string name = names != null ? i < names.Length ? names[i] : default : default;
                //var memStream = new MemoryStream();
                //stream.CopyTo(memStream);
                var streamContent = new StreamContent(stream);
                var fStream = stream as FileStream;
                var fileName = fStream != null ? fStream.Name : "null";
                if (name != null)
                {
                    streamContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data")
                    {
                        Name = name,
                        FileName = fileName
                    };
                    form.Add(streamContent, name);
                }

                else form.Add(streamContent);
            }

            return form;
        }

        private HttpContent MultiPartForm(byte[] bytes, string name)
        {
            var form = new MultipartFormDataContent();
            var bytesContent = new ByteArrayContent(bytes);
            //bytesContent.Headers.ContentType = MediaTypeHeaderValue.Parse("multipart/form-data");
            bytesContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data")
            {
                Name = name
            };
            form.Add(bytesContent, name);

            return form;
        }

        private async Task<T> Deserialize<T>(string data)
        {
            return await Task.Factory.StartNew(() => JsonConvert.DeserializeObject<T>(data));
        }

        private bool DoesBodyObjectContainBytesOrStreams(object bodyObject, out HttpContent content)
        {
            content = default;

            if (bodyObject == null)
                return false;

            var properties = bodyObject.GetType().GetProperties();

            if (properties == null || properties == default)
                return false;

            var hasBytesOrStreams = false;
            var form = new MultipartFormDataContent();

            foreach (var property in properties)
            {
                var value = property.GetValue(bodyObject);

                if(TryParse(value, out Stream stream))
                {
                    var streamContent = new StreamContent(stream);
                    //streamContent.Headers.ContentType = MediaTypeHeaderValue.Parse("multipart/form-data");
                    var fileStream = stream as FileStream;
                    var name = fileStream != null ? fileStream.Name : property.Name;
                    streamContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data")
                    {
                        Name = property.Name,
                        FileName = name
                    };
                    form.Add(streamContent, property.Name);
                    hasBytesOrStreams = true;
                }
                else if(value is byte[] bytes)
                {
                    var bytesContent = new ByteArrayContent(bytes);
                    //bytesContent.Headers.ContentType = MediaTypeHeaderValue.Parse("multipart/form-data");
                    bytesContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data")
                    {
                        Name = property.Name,
                        FileName = property.Name
                    };
                    form.Add(bytesContent, property.Name);
                    hasBytesOrStreams = true;
                }else
                {
                    var stringContent = new StringContent(SerializeObject(value));
                    //stringContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                    stringContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data")
                    {
                        Name = property.Name
                    };
                    form.Add(stringContent, property.Name);
                }
            }

            if (hasBytesOrStreams)
                content = form;

            return hasBytesOrStreams;
        }

        private bool TryParse<T, TParse>(T arg, out TParse parse) where TParse : class
        {
            parse = arg as TParse;

            return parse != null;
        }

        public async Task<Response> Execute()
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

                            var bodyArg = arguments[i];
                            HttpContent content = default;
                            if (TryParse(bodyArg, out Stream stream))
                            {
                                bodyContent = MultiPartForm(stream, body.Name);
                            }else if(TryParse(bodyArg, out List<Stream> streams))
                            {
                                bodyContent = MultiPartForm(streams, new[] { body.Name });
                            }
                            else if (bodyArg is byte[] bytes)
                            {
                                bodyContent = MultiPartForm(bytes, body.Name);
                            }
                            else if (DoesBodyObjectContainBytesOrStreams(bodyArg, out content))
                            {
                                bodyContent = content;
                            }
                            else
                            {
                                bodyContent = Serialize(bodyArg);
                            }
                        }
                        else if (attr is Header header)
                        {
                            if (header.HasAuthModel)
                                headersParams.Add(header.Key, header.Authentication.ToString());
                            else if (!string.IsNullOrWhiteSpace(header.Value))
                                headersParams.Add(header.Key, header.Value);
                            else
                            {
                                var value = arguments[i] as string;
                                if(!string.IsNullOrWhiteSpace(value))
                                {
                                    headersParams.Add(header.Key, value);
                                }
                            }
                            continue;
                        }
                        else if (attr is Param param)
                        {
                            string _param = arguments[i].ToString();
                            uri = new Uri($@"{uri.AbsoluteUri}/{_param}");
                        }
                    }
                }
            }

            if(bodyContent == default)
            {
                bodyContent = new StringContent("");
            }
            // add the headers to the string content

            if (!recycle)
                client = new HttpClient(RestService._handler);

            foreach (var kp in headersParams)
            {
                client.DefaultRequestHeaders.Add(kp.Key, kp.Value);
            }

            Response response;

            using var requestM = new HttpRequestMessage();

            try
            {
                if (HttpMethod != HttpMethod.Get)
                    requestM.Content = bodyContent;
                requestM.RequestUri = uri;
                requestM.Method = HttpMethod;

                HttpResponseMessage responseM = await client.SendAsync(requestM);

                if (responseM != null)
                {
                    var bytes = await responseM.Content.ReadAsByteArrayAsync();
                    response = new Response(bytes, responseM);
                }
                else
                    response = new Response(Array.Empty<byte>(), responseM);

            }
            catch (Exception e)
            {
                response = new Response(Encoding.UTF8.GetBytes(e.Message), default);
            }

            uri = new Uri(url);

            return response;
        }

    }
}
