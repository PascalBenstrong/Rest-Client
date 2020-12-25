using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace TheProcessE.RestApiClient
{
    public partial class ServiceMethodInfo
    {

        private static HttpContent Serialize<T>(T data)
        {
            var json = SerializeObject(data);
            var content = new StringContent(json);

            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            return content;
        }

        private static string SerializeObject<TValue>(TValue data) => JsonSerializer.Serialize(data);

        private static HttpContent MultiPartForm(Stream stream, string name)
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

        private static HttpContent MultiPartForm(List<Stream> streams, string[] names)
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

        private static HttpContent MultiPartForm(byte[] bytes, string name)
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

        private static bool DoesBodyObjectContainBytesOrStreams(object bodyObject, out HttpContent content)
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

                if (TryParse(value, out Stream stream))
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
                else if (value is byte[] bytes)
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
                }
                else
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

        private static bool TryParse<T, TParse>(T arg, out TParse parse) where TParse : class
        {
            parse = arg as TParse;

            return parse != default(TParse);
        }

        private static string ParseParams(string url, ref PARAM param, int index, ref object[] arguments)
        {
            string _param = arguments[index]?.ToString();

            if (!string.IsNullOrWhiteSpace(_param))
            {
                if (param.IsNamed)
                {
                    url = url.Replace("{" + param.Name + "}", _param);
                }
                else
                {
                    url = $"{url}/{_param}";
                }
            }

            return url;
        }

        private static void ParseHeader(IDictionary<string,string> headersParams, ref HEADER header, int index, ref object[] arguments, bool isMethodHeader = false)
        {
            if (header.HasAuthModel)
                headersParams.Add(header.Key, header.Authentication.ToString());
            else if (!string.IsNullOrWhiteSpace(header.Value))
                headersParams.Add(header.Key, header.Value);
            else if(isMethodHeader)
            {
                var value = arguments[index] as string;
                if (!string.IsNullOrWhiteSpace(value))
                {
                    headersParams.Add(header.Key, value);
                }
            }
        }
        private void HandleBodyContent(ref HttpContent bodyContent, int i, ref BODY body)
        {
            var bodyArg = arguments[i];
            //HttpContent content = default;
            if (TryParse(bodyArg, out Stream stream))
            {
                bodyContent = MultiPartForm(stream, body.Name);
            }
            else if (TryParse(bodyArg, out List<Stream> streams))
            {
                bodyContent = MultiPartForm(streams, new[] { body.Name });
            }
            else if (bodyArg is byte[] bytes)
            {
                bodyContent = MultiPartForm(bytes, body.Name);
            }
            else if (DoesBodyObjectContainBytesOrStreams(bodyArg, out bodyContent))
            {
                //bodyContent = content;
            }
            else
            {
                bodyContent = Serialize(bodyArg);
            }
        }
    }
}
