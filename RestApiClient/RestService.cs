
using System;
using System.Collections.Generic;

namespace TheProcessE.RestApiClient
{
    public static class RestService
    {
        private static object Service = null;
        public static T GetService<T>() where T : class
        {
            if (!IsServiceInterface(typeof(T)))
                throw new ArgumentException("The generic type must be an interface and must not extend other interfaces");

            if(Service != null)
            return (T)Service;

            Service = CreateNewService<T>();

            return (T)Service ;
        }

        private static T CreateNewService<T>() where T: class
        {
            return RuntimeProxy.Create<T>();
        }

        private static bool IsServiceInterface(Type service)
        {
            if (!service.IsInterface)
                return false;

            return service.GetInterfaces().Length <= 0;
        }

        public static IReadOnlyDictionary<Type, object> ParseClassParams(Type service)
        {
            Dictionary<Type, object> result = new Dictionary<Type, object>();

            var attributes = Attribute.GetCustomAttributes(service);

            foreach (var attribute in attributes)
            {
                if (attribute is URL)
                {
                    var instance = (URL)attribute;
                    result.Add(typeof(URL), instance.Path);
                }
            }

            return result;
        }
    }
}
