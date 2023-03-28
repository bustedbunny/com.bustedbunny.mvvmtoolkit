using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;


namespace MVVMToolkit.DependencyInjection
{
    public partial class ServiceProvider : IServiceProvider
    {
        private readonly Dictionary<Type, object> _serviceMap = new();

        public void RegisterService(object service)
        {
            _serviceMap.Add(service.GetType(), service);
        }

        public void RegisterService<T>(object service)
        {
            if (service is not T)
            {
                throw new($"Tried to register service of type {service.GetType().Name} under type {typeof(T).Name}.");
            }

            _serviceMap.Add(typeof(T), service);
        }

        public void UnregisterService(object service)
        {
            _serviceMap.Remove(service.GetType());
        }

        public void UnregisterService<T>()
        {
            _serviceMap.Remove(typeof(T));
        }

        public object GetService(Type serviceType)
        {
            return _serviceMap[serviceType];
        }

        public T GetService<T>()
        {
            return (T)_serviceMap[typeof(T)];
        }

        public void Inject()
        {
            const BindingFlags flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
            foreach (var (_, obj) in _serviceMap)
            {
                var type = obj.GetType();
                if (!FieldMap.TryGetValue(type.FullName, out var fields))
                {
                    continue;
                }

                foreach (var field in fields)
                {
                    var fieldInfo = type.GetField(field, flags);
                    var fieldType = fieldInfo.FieldType;
                    if (!_serviceMap.TryGetValue(fieldType, out var service))
                    {
                        Debug.LogError(
                            $"Couldn't find service of type {fieldType.Name} to inject into {type.Name}");
                        continue;
                    }

                    fieldInfo.SetValue(obj, service);
                }
            }
        }
    }
}