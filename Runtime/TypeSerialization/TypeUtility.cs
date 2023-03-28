using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace MVVMToolkit.TypeSerialization
{
    internal static class TypeUtility
    {
        static TypeUtility()
        {
            AssemblyMap = new();

            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                AssemblyMap.Add(assembly.GetName().Name, assembly);
            }
        }

        private static readonly Dictionary<string, Assembly> AssemblyMap;

        public static List<Type> GetTypes(Type derivingType)
        {
            var result = new List<Type>();
            var assets = Resources.LoadAll<TextAsset>($"MVVMToolkit/TypeCache/{derivingType.Name}");
            if (assets is null)
            {
                return result;
            }

            foreach (var textAsset in assets)
            {
                var types = JsonUtility.FromJson<SerializedTypes>(textAsset.text);
                var assembly = AssemblyMap[types.assemblyName];
                foreach (var typeName in types.fullTypeNames)
                {
                    result.Add(assembly.GetType(typeName));
                }
            }

            return result;
        }
    }
}