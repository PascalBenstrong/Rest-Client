
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net.Http;

namespace TheProcessE.RestApiClient
{
    public static class RestService
    {
        private static readonly ConcurrentDictionary<string, ServiceInfo> cachedRestClients = new ConcurrentDictionary<string, ServiceInfo>();


        /// <summary>
        /// Creates a new Rest Api Service of the type <typeparamref name="T"/> if none exists in cache.
        /// </summary>
        /// <typeparam name="T"/>
        /// <param name="recycle">when true, reuses the HttpClient used by this RestApi</param>
        /// <param name="client"> HttpClient to be used on all RestServices of type <typeparamref name="T"/></param>
        /// <returns><typeparamref name="T"/></returns>

        public static T GetService<T>(HttpClient client = default) where T : class
        {
            var service = CreateOrGetServiceInfo<T>(client);

            return service.GetServiceInfo<T>();
        }

        /// <summary>
        /// Creates a new Rest Api Service of the type <typeparamref name="T"/> if none exists in cache.
        /// Use <see cref="GetService{T}(HttpClient)"/> to get an instance of an api service that is created using this method.
        /// </summary>
        /// <typeparam name="T"/>
        /// <param name="recycle">when true, reuses the HttpClient used by this RestApi</param>
        /// <param name="client"> HttpClient to be used on all RestServices of type <typeparamref name="T"/></param>
        /// <returns>void</returns>

        public static void CreateService<T>(HttpClient client = default) where T : class
        {
            CreateOrGetServiceInfo<T>(client);
        }

        private static ServiceInfo CreateOrGetServiceInfo<T>(HttpClient client) where T : class
        {
            if (!IsServiceInterface(typeof(T)))
                throw new ArgumentException("The generic type must be an interface and must not extend other interfaces");

            var typeName = typeof(T).AssemblyQualifiedName;
            return cachedRestClients.GetOrAdd(typeName, CreateNewService<T>(client));
        }

        private static ServiceInfo CreateNewService<T>(HttpClient client) where T: class
        {
            if (client == default)
            {
                client = new HttpClient();
            }
            return new ServiceInfo<T>(client);
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
                }else if( attribute is HEADER header)
                {
                    result.Add(typeof(HEADER), header);
                }
            }

            return result;
        }
    }
}
