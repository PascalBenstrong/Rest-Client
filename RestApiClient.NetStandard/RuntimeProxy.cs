using System.Net.Http;
using System.Reflection;

namespace TheProcessE.RestApiClient
{
    public abstract class RuntimeProxy
    {
        internal static Target Create<Target>(HttpClient client) where Target : class
        {
            return InternalProxy<Target>.Create(client);
        }

        public class InternalProxy<Target> : DispatchProxy where Target : class
        {
            private HttpClient Client;
            internal static Target Create(HttpClient client)
            {
                var target = Create<Target, InternalProxy<Target>>();

                var proxy = target as InternalProxy<Target>;

                proxy.Client = client;

                return target;
            }

            protected override object Invoke(MethodInfo targetMethod, object[] args)
            {

                var serviceMethodInfo = ServiceMethodInfo.CreateOrAdd<Target>(targetMethod, args, Client);

                return serviceMethodInfo.Execute();
            }
        }

    }
}
