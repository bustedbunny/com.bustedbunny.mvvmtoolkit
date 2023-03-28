using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices;
using MVVMToolkit.TypeSerialization;
using Unity.Burst;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;

namespace MVVMToolkit.Messaging
{
    public static class TypeManager
    {
        private static Dictionary<long, TypeInfo> _typeMap;

        public static TypeInfo GetTypeInfo(long hash) => _typeMap[hash];

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
        private static void Init() => Initialize();

        private static void Initialize()
        {
            _typeMap = new();

            foreach (var type in TypeUtility.GetTypes(typeof(IUnmanagedMessage)))
            {
                if (!UnsafeUtility.IsUnmanaged(type))
                {
                    Debug.LogError(
                        $"Type {type.Name} contains reference types. Only unmanaged IUnmanagedMessage supported.");
                }

                var typeHash = BurstRuntime.GetHashCode64(type);

                var size = Marshal.SizeOf(type);
                var wrapperType = typeof(Wrapped<>).MakeGenericType(type);

                const BindingFlags flags = BindingFlags.Static | BindingFlags.NonPublic;

                var method = wrapperType.GetMethod("GetDelegate", flags);

                if (method is null)
                {
                    Debug.LogError($"Type {wrapperType.Name} does not contain GetDelegate method.");
                    continue;
                }


                var constructor = (Func<IntPtr, object>)method.Invoke(null, null);

                var typeInfo = new TypeInfo(size, wrapperType, type, constructor);

                _typeMap.Add(typeHash, typeInfo);
            }
        }
    }
}