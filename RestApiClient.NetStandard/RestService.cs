
using System;
using System.Collections.Generic;
using System.Net.Http;

namespace TheProcessE.RestApiClient
{
    public static class RestService
    {
        private static object Service = null;
        internal static readonly HttpClientHandler _handler = new HttpClientHandler()
        {
            ServerCertificateCustomValidationCallback = delegate { return true; }
        };
        private static HttpClient _client = new HttpClient(_handler);
        private static HttpClient Client => _client ??= new HttpClient(_handler);
        /// <summary>
        /// Creates a new Rest Api of the type <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="recycle">when true, reuses the HttpClient used by this RestApi</param>
        /// <returns><typeparamref name="T"/></returns>
        public static T GetService<T>(bool recycle = false) where T : class
        {
            if (!IsServiceInterface(typeof(T)))
                throw new ArgumentException("The generic type must be an interface and must not extend other interfaces");

            if(Service != null && Service.GetType().IsAssignableFrom(typeof(T)))
            return (T)Service;

            Service = CreateNewService<T>(recycle);

            return (T)Service ;
        }

        private static T CreateNewService<T>(bool recycle) where T: class
        {
            return RuntimeProxy.Create<T>(Client, recycle);
        }

        private static bool IsServiceInterface(Type service)
        {
            if (!service.IsInterface)
                return false;

            return service.GetInterfaces().Length <= 0;
        }

        private static IReadOnlyDictionary<Type, object> ParseClassParams(Type service)
        {
            Dictionary<Type, object> result = new Dictionary<Type, object>();

            var attributes = Attribute.GetCustomAttributes(service);

            foreach (var attribute in attributes)
            {
                if (attribute is URL instance)
                {
                    result.Add(typeof(URL), instance.Path);
                }else if( attribute is Header header)
                {
                    result.Add(typeof(Header), header);
                }
            }

            return result;
        }
    }
}
