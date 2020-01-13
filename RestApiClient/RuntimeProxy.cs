using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Runtime.Remoting.Messaging;
using System.Runtime.Remoting.Proxies;
using System.Text;
using System.Threading.Tasks;

namespace TheProcessE.RestApiClient
{
    internal abstract class RuntimeProxy
    {
        public static readonly object Default = new object();

        internal static Target Create<Target>() where Target : class
        {
            return (Target)new InternalProxy<Target>().GetTransparentProxy();
        }

        class InternalProxy<Target> : RealProxy where Target : class
        {
            public InternalProxy() : base (typeof(Target))
            {
            }
            public override IMessage Invoke(IMessage msg)
            {
                var methodCall = (IMethodCallMessage)msg;
                var method = (MethodInfo)methodCall.MethodBase;

                try
                {
                    var serviceMethodInfo = ServiceMethodInfo.CreateOrAdd<Target>(method, methodCall.Args);
                    var result = method.ReturnType == typeof(Task<Response>) ? serviceMethodInfo.Execute() : (object)serviceMethodInfo.Execute().Result;
                    return new ReturnMessage(result, null, 0, methodCall.LogicalCallContext, methodCall);
                }
                catch (Exception ex)
                {
                    if (ex is TargetInvocationException && ex.InnerException != null)
                        return new ReturnMessage(ex.InnerException, msg as IMethodCallMessage);

                    return new ReturnMessage(ex, msg as IMethodCallMessage);
                }
            }
        }

    }
}
