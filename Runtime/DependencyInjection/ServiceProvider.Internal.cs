using System.Collections.Generic;
using System.Runtime.CompilerServices;
using MVVMToolkit.TypeSerialization;
using UnityEngine;

[assembly: InternalsVisibleTo("MVVMToolkit.Editor")]

namespace MVVMToolkit.DependencyInjection
{
    public partial class ServiceProvider
    {
        internal const string FolderName = "DI";
        internal const string FileName = "FieldMap";

        static ServiceProvider()
        {
            var rawData = Resources.Load<TextAsset>($"MVVMToolkit/{FileName}");
            var fieldsMap = JsonUtility.FromJson<SerializedFields>(rawData.text);

            FieldMap = new(fieldsMap.items.Count);

            foreach (var data in fieldsMap.items)
            {
                FieldMap.Add(data.fullTypeName, data.fieldNames);
            }
        }

        private static readonly Dictionary<string, List<string>> FieldMap;
    }
}