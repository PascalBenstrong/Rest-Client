namespace TheProcessE.RestApiClient
{
    public abstract class RuntimeProxyInterceptor
    {
        public virtual object Invoke(RuntimeProxyInvoker invoker)
        {
            return invoker.Invoke();
        }
    }
}
