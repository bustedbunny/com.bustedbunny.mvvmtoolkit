using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;


namespace MVVMToolkit.DependencyInjection
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

        public void Inject()
        {
            const BindingFlags flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
            foreach (var (type, obj) in _serviceMap)
            {
                foreach (var field in type.GetFields(flags))
                {
                    if (field.GetCustomAttribute<InjectAttribute>() == null) continue;
                    var fieldType = field.FieldType;
                    if (!_serviceMap.TryGetValue(fieldType, out var service))
                    {
                        Debug.LogError(
                            $"Couldn't find service of type {fieldType.Name} to inject into {type.Name}");
                        continue;
                    }

                    field.SetValue(obj, service);
                }
            }
        }
    }
}