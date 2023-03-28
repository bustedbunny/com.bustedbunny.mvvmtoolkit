#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace MVVMToolkit.Messaging
{
    public static partial class TypeManager
    {
        private const string FolderName = "BurstWrapperTypeCache";

        [InitializeOnLoadMethod]
        private static void CacheTypes()
        {
            var assemblyMap = new Dictionary<Assembly, List<string>>();

            foreach (var type in TypeCache.GetTypesDerivedFrom(typeof(IUnmanagedMessage)))
            {
                if (!assemblyMap.TryGetValue(type.Assembly, out var list))
                {
                    list = new();
                    assemblyMap[type.Assembly] = list;
                }

                list.Add(type.FullName);
            }

            var path = Path.Combine(Application.dataPath, "Resources", "MVVMToolkit", FolderName);
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            foreach (var (assembly, list) in assemblyMap)
            {
                var fileName = assembly.GetName().Name;
                var json = JsonUtility.ToJson(new SerializedTypes { fullTypeNames = list });
                var filePath = Path.Combine(path, fileName + ".txt");
                File.WriteAllText(filePath, json);
            }

            AssetDatabase.ImportAsset($"Assets/Resources/MVVMToolkit/{FolderName}", ImportAssetOptions.ImportRecursive);
        }
    }
}
#endif