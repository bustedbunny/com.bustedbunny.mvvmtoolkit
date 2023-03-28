using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices;
using Unity.Burst;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;

namespace MVVMToolkit.Messaging
{
    public static partial class TypeManager
    {
        private const string FolderName = "BurstWrapperTypeCache";
        private static bool _initialized;

        private static Dictionary<long, TypeInfo> _typeMap;

        public static TypeInfo GetTypeInfo(long hash) => _typeMap[hash];

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
        private static void Init() => Initialize();

        private static void Initialize()
        {
            _typeMap = new();


            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                var path = $"MVVMToolkit/{FolderName}/{assembly.GetName().Name}";
                var json = Resources.Load<TextAsset>(path);

                if (json is null)
                {
                    continue;
                }

                var types = JsonUtility.FromJson<SerializedTypes>(json.text);

                foreach (var typeName in types.fullTypeNames)
                {
                    var type = assembly.GetType(typeName);


                    if (type.IsAbstract || !typeof(IUnmanagedMessage).IsAssignableFrom(type)) continue;
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
}