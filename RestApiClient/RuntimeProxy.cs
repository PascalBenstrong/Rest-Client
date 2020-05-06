using System;
using System.Reflection;
using System.Threading.Tasks;

namespace TheProcessE.RestApiClient
{
    public abstract class RuntimeProxy
    {
        public static readonly object Default = new object();

        internal static Target Create<Target>() where Target : class
        {
            return InternalProxy<Target>.Create();
        }

        public class InternalProxy<Target> : DispatchProxy where Target : class
        {
            internal static Target Create()
            {
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
                    var serviceMethodInfo = ServiceMethodInfo.CreateOrAdd<Target>(targetMethod, args);
                    var result = targetMethod.ReturnType == typeof(Task<Response>) ? serviceMethodInfo.Execute() : (object)serviceMethodInfo.Execute().Result;
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
