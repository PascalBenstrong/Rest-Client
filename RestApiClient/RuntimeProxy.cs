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

        internal static Target Create<Target>(Target instance, RuntimeProxyInterceptor interceptor) where Target : class
        {
            return (Target)new InternalProxy<Target>(instance, interceptor).GetTransparentProxy();
        }

        /*        public static Target Create<Target>(Target instance, Func<RuntimeProxyInvoker, object> factory) where Target : class
                {
                    return (Target)new InternalProxy<Target>(instance, new InternalRuntimeProxyInterceptor(factory)).GetTransparentProxy();
                }

                public static Target Create<Target>(Func<RuntimeProxyInvoker, object> factory) where Target : class
                {
                    return (Target)new InternalProxy<Target>(new InternalRuntimeProxyInterceptor(factory)).GetTransparentProxy();
                }*/


        internal static Target Create<Target>() where Target : class
        {
            return (Target)new InternalProxy<Target>().GetTransparentProxy();
        }


        class InternalProxy<Target> : RealProxy where Target : class
        {
            readonly Target Instance;
            readonly RuntimeProxyInterceptor Interceptor;

            public InternalProxy(Target instance, RuntimeProxyInterceptor interceptor)
                : base(typeof(Target))
            {
                Instance = instance;
                Interceptor = interceptor;
            }

            public InternalProxy() : base (typeof(Target))
            {
                Instance = null;
                Interceptor = null;
            }

            public override IMessage Invoke(IMessage msg)
            {
                var methodCall = (IMethodCallMessage)msg;
                var method = (MethodInfo)methodCall.MethodBase;

                try
                {
                    //var result = Interceptor.Invoke(new InternalRuntimeProxyInterceptorInvoker(Instance, method, methodCall.InArgs));

                    var serviceMethodInfo = ServiceMethodInfo.CreateOrAdd<Target>(method, methodCall.Args);
                    /*                    for(int i = 0; i < ps.Length; i++)
                                        {
                                            if(i < args.Length)
                                            {
                                                foreach(var att in ps[i].GetCustomAttributes())
                                                {
                                                    if(att is Header)
                                                    {
                                                        var header = att as Header;
                                                        var arg = args[i];
                                                        Console.WriteLine($"Params and Headers: {header.header.Key}\tvalue: {arg}");
                                                    }
                                                }
                                            }
                                        }*/

                    //Console.WriteLine($"Result: {(ps[0].Member.GetCustomAttributes().Single() as POST).Path}");

                    /*                    if (result == Default)
                                            result = method.ReturnType.IsPrimitive ? Activator.CreateInstance(method.ReturnType) : null;*/

                    var result = serviceMethodInfo.Execute();
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

/*        class InternalRuntimeProxyInterceptor : RuntimeProxyInterceptor
        {
            readonly Func<RuntimeProxyInvoker, object> Factory;

            public InternalRuntimeProxyInterceptor(Func<RuntimeProxyInvoker, object> factory)
            {
                this.Factory = factory;
            }

            public override object Invoke(RuntimeProxyInvoker invoker)
            {
                return Factory(invoker);
            }
        }

        class InternalRuntimeProxyInterceptorInvoker : RuntimeProxyInvoker
        {
            public InternalRuntimeProxyInterceptorInvoker(object target, MethodInfo method, object[] args)
                : base(target, method, args)
            { }
        }*/
    }
}
