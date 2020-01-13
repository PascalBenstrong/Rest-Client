using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;

namespace TheProcessE.RestApiClient
{
    public abstract class RuntimeProxyInvoker
    {
        public readonly object Target;
        public readonly MethodInfo Method;
        public readonly ReadOnlyCollection<object> Arguments;

        public RuntimeProxyInvoker(object target, MethodInfo method, object[] args)
        {
            this.Target = target;
            this.Method = method;
            this.Arguments = new ReadOnlyCollection<object>(args);
        }

        public virtual object Invoke()
        {
            return Invoke(this.Target);
        }

        public object Invoke(object target)
        {
            if (target == null)
                throw new ArgumentNullException("target is null.");

            try
            {
                return this.Method.Invoke(target, this.Arguments.ToArray());
                //return "From invoke method";
            }
            catch (TargetInvocationException ex)
            {
                throw ex.InnerException;
            }

        }
    }
}
