using System;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;

namespace TheProcessE.RestApiClient
{
    public abstract class RuntimeProxy
    {
        public static readonly object Default = new object();
        private static HttpClient _client { get; set; }
        private static bool _recycle { get; set; }

        internal static Target Create<Target>(HttpClient client, bool recycle) where Target : class
        {
            _client = client;
            _recycle = recycle;
            return InternalProxy<Target>.Create(client, recycle);
        }

        public class InternalProxy<Target> : DispatchProxy where Target : class
        {
            
            internal static Target Create(HttpClient client, bool recycle)
            {
                _client = client;
                _recycle = recycle;
                var proxy = Create<Target, InternalProxy<Target>>();

                return proxy;
            }

            protected override object Invoke(MethodInfo targetMethod, object[] args)
            {
                //var methodCall = (IMethodCallMessage)msg;
                //var method = (MethodInfo)methodCall.MethodBase;
                //var method = (MethodInfo)methodCall.MethodBase;

                try
                {
                    var serviceMethodInfo = ServiceMethodInfo.CreateOrAdd<Target>(targetMethod, args, _client, _recycle);
                    var result = targetMethod.ReturnType == typeof(Task<Response>) ? serviceMethodInfo.Execute() : (object)Task.Factory.StartNew(async () => await serviceMethodInfo.Execute()).Unwrap().GetAwaiter().GetResult();
                    return result;
                }
                catch (Exception ex)
                {
                    if (ex is TargetInvocationException && ex.InnerException != null)
                        throw new Exception(ex.Message, ex.InnerException);

                    throw new Exception(ex.Message);
                }
            }
        }

    }
}
