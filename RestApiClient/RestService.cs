
using System;
using System.Collections.Generic;

namespace TheProcessE.RestApiClient
{
    public static class RestService
    {
        private static object Service = null;
        public static T getService<T>() where T : class
        {
            if (!isServiceInterface(typeof(T)))
                throw new ArgumentException("The generic type must be an interface and must not extend other interfaces");

            if(Service != null)
            return (T)Service;

            Service = CreateNewService<T>();

            return (T)Service ;
        }

        private static T CreateNewService<T>() where T: class
        {
            //dynamic instance = new ExpandoObject();

            //instance.test = (Func<string>)(() => "Hello");

            //var attr = typeof(T).GetCustomAttributes();

/*            var methodInfos = typeof(T).GetMethods();
            foreach (var m in methodInfos)
            {
                AddMethods(instance, m.Name);
            }
            T result = Impromptu.ActLike<T>(instance);*/
            return RuntimeProxy.Create<T>();
        }

/*        public static void AddMethods(ExpandoObject expando, string propertyName)
        {
            var f = (Func <object, Task < Response<string> >>)(async (o) =>
            {
                var atts = Attribute.GetCustomAttributes(o.GetType());
                Console.WriteLine($"input: {o}");
                foreach(var att in atts)
                {
                    Console.WriteLine($"input: {att}");
                }
                return new Response<string>("Hello", default);
            });
            // ExpandoObject supports IDictionary so we can extend it like this
            var expandoDict = expando as IDictionary<string, object>;
            if (expandoDict.ContainsKey(propertyName))
                expandoDict[propertyName] = f;
            else
                expandoDict.Add(propertyName, f);
        }
*/
        private static bool isServiceInterface(Type service)
        {
            if (!service.IsInterface)
                return false;

            return service.GetInterfaces().Length <= 0;
        }

        public static IReadOnlyDictionary<Type, object> parseClassParams(Type service)
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
