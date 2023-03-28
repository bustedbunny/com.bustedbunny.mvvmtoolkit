using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using MVVMToolkit.TypeSerialization;
using UnityEditor;
using UnityEngine;

namespace MVVMToolkit.Editor.TypeSerialization
{
    public static class TypeCaching
    {
        public static void CacheTypes(Type deriving)
        {
            var assemblyMap = new Dictionary<Assembly, List<string>>();

            foreach (var type in TypeCache.GetTypesDerivedFrom(deriving))
            {
                if (type.IsAbstract)
                {
                    continue;
                }

                if (!assemblyMap.TryGetValue(type.Assembly, out var list))
                {
                    list = new();
                    assemblyMap[type.Assembly] = list;
                }

                list.Add(type.FullName);
            }

            var path = Path.Combine(Application.dataPath, "Resources", "MVVMToolkit", "TypeCache", deriving.Name);
            if (Directory.Exists(path))
            {
                Directory.Delete(path, true);
            }

            Directory.CreateDirectory(path);


            foreach (var (assembly, list) in assemblyMap)
            {
                var fileName = assembly.GetName().Name;
                var json = JsonUtility.ToJson(new SerializedTypes
                {
                    assemblyName = fileName,
                    fullTypeNames = list,
                });
                var filePath = Path.Combine(path, fileName + ".txt");
                File.WriteAllText(filePath, json);
            }

            AssetDatabase.ImportAsset($"Assets/Resources/MVVMToolkit/TypeCache/{deriving.Name}",
                ImportAssetOptions.ImportRecursive);
        }
    }
}