
using System;
using System.Collections.Generic;
using System.Net.Http;

namespace TheProcessE.RestApiClient
{
    internal struct ServiceInfo<SType> where SType : class
    {
        internal SType Service { get; private set; }
        internal readonly HttpClient Client;

        internal ServiceInfo(HttpClient client)
        {
            Service = default;
            Client = client;
        }

        private SType LazyLoad()
        {
            return RuntimeProxy.Create<SType>(Client);
        }

        public SType GetService()
        {
            if(Service == null)
            {
                Service = LazyLoad();
            }

            return Service;
        }

        public static implicit operator SType(ServiceInfo<SType> serviceInfo)
        {
            return serviceInfo.GetService();
        }

        public static implicit operator ServiceInfo(ServiceInfo<SType> serviceInfo)
        {
            return new ServiceInfo(serviceInfo);
        }

    }

    internal struct ServiceInfo
    {

        private readonly object Service;

        internal ServiceInfo(object service)
        {
            Service = service;
        }

        internal T GetServiceInfo<T>() where T: class
        {
            var service = (ServiceInfo<T>)Service;
            return service;
        }
    }

}
