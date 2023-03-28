using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using MVVMToolkit.DependencyInjection;
using MVVMToolkit.TypeSerialization;
using UnityEditor;
using UnityEngine;

namespace MVVMToolkit.Editor.DependencyInjection
{
    public static class FieldCache
    {
        [InitializeOnLoadMethod]
        private static void CacheFields()
        {
            var map = new Dictionary<Type, List<FieldInfo>>();

            foreach (var field in TypeCache.GetFieldsWithAttribute<InjectAttribute>())
            {
                if (!map.TryGetValue(field.ReflectedType, out var list))
                {
                    list = new();
                    map[field.ReflectedType] = list;
                }

                list.Add(field);
            }

            var result = new List<SerializedField>(map.Count);
            foreach (var (type, list) in map)
            {
                var fields = new List<string>(list.Count);

                foreach (var field in list)
                {
                    fields.Add(field.Name);
                }

                result.Add(new()
                {
                    fullTypeName = type.FullName,
                    fieldNames = fields
                });
            }

            var path = Path.Combine(Application.dataPath, "Resources", "MVVMToolkit", ServiceProvider.FolderName);
            if (Directory.Exists(path))
            {
                Directory.Delete(path, true);
            }

            Directory.CreateDirectory(path);

            var json = JsonUtility.ToJson(new SerializedFields { items = result });
            File.WriteAllText(Path.Combine(path, $"{ServiceProvider.FileName}.txt"), json);


            AssetDatabase.ImportAsset(
                $"Assets/Resources/MVVMToolkit/{ServiceProvider.FolderName}/{ServiceProvider.FileName}.txt");
        }
    }
}