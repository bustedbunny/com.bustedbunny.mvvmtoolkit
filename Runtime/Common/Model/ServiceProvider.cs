using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace MVVMToolkit
{
    public class ServiceProvider : IServiceProvider
    {
        private readonly Dictionary<Type, object> _serviceMap = new();

        public void RegisterService(object service)
        {
            _serviceMap.Add(service.GetType(), service);
        }

        public object GetService(Type serviceType)
        {
            return _serviceMap[serviceType];
        }

        public T GetService<T>()
        {
            return (T)_serviceMap[typeof(T)];
        }
    }
}