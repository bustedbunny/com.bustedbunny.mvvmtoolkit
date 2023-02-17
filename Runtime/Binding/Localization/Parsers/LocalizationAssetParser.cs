using System;
using MVVMToolkit.Binding.Generics;
using UnityEngine.AddressableAssets;
using UnityEngine.Localization;
using UnityEngine.Localization.Tables;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

namespace MVVMToolkit.Binding.Localization
{
    public class LocalizationAssetParser : BindingParser<LocalizedAssetBinding>
    {
        private readonly LocalizedAssetTable[] _assetTables;

        public LocalizationAssetParser(LocalizedAssetTable[] assetTables) : base(null)
        {
            _assetTables = assetTables;
        }

        public override char Symbol() => '#';

        public override void Process(VisualElement element, string key)
        {
            var separator = key.IndexOf('>');
            var assetKey = key[..separator];
            separator++;
            var bindingKey = key[separator..];

            var table = GetTable(assetKey);

            BindingUtility.GetTargetObject(element, bindingKey, out var setTarget, out var setPropName);
            var setProp = PropertyUtility.GetSetProperty(setTarget, setPropName);

            boundingMap.Add(ConversionUtility.Binding(setProp, setTarget, table, assetKey), key);
        }

        private LocalizedAssetTable GetTable(string assetKey)
        {
            foreach (var assetTable in _assetTables)
            {
                if (!assetTable.TableReference.SharedTableData.Contains(assetKey)) continue;
                return assetTable;
            }

            throw new BindingException($"No matching table found for key {assetKey}");
        }
    }

    public abstract class LocalizedAssetBinding : IElementBinding
    {
        public abstract void Unbind();
    }

    public class LocalizedAssetBinding<T> : LocalizedAssetBinding where T : Object
    {
        private readonly LocalizedAssetTable _table;
        private readonly Action<Object> _operation;
        private readonly string _assetKey;

        public LocalizedAssetBinding(LocalizedAssetTable table, string key, Action<Object> operation)
        {
            _table = table;
            _assetKey = key;
            _operation = operation;
            _table.TableChanged += OnTableChange;


            var loading = _table.CurrentLoadingOperationHandle;
            if (loading.IsDone) OnTableChange(loading.Result);
        }

        private AsyncOperationHandle<T> _curOperation;


        private void OnTableChange(AssetTable value)
        {
            _curOperation = Addressables.LoadAssetAsync<T>(value[_assetKey].Guid);
            if (_curOperation.IsDone)
            {
                CurOperationOnCompleted(_curOperation);
            }
            else
            {
                _curOperation.Completed += CurOperationOnCompleted;
            }
        }

        private void CurOperationOnCompleted(AsyncOperationHandle<T> obj)
        {
            _operation(obj.Result);
        }

        public override void Unbind()
        {
            _table.TableChanged -= OnTableChange;
        }
    }
}