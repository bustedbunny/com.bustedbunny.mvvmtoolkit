using System;
using System.ComponentModel;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.Localization.Tables;
using UnityEngine.UIElements;

namespace MVVMToolkit.Binding.Localization
{
    public class LocalizedTextBinding : IElementBinding
    {
        private readonly TextElement _element;
        private readonly Action<VisualElement, string> _operation;
        private LocalizedStringBinding _stringBinding;
        private readonly CancellationTokenSource _cts;

        public LocalizedTextBinding(TextElement element, INotifyPropertyChanged binding, string key,
            LocalizedStringTable[] tables,
            Action<VisualElement, string> operation)
        {
            _operation = operation;
            _element = element;

            _cts = new();
            BindAsync(binding, key, tables, _cts.Token).Forget();
        }

        // Binding operation has to be async in case LocalizationSettings are not loaded yet
        private async UniTaskVoid BindAsync(INotifyPropertyChanged binding, string key,
            LocalizedStringTable[] tables, CancellationToken ct)
        {
            var operation = LocalizationSettings.InitializationOperation;
            while (!operation.IsDone)
            {
                if (ct.IsCancellationRequested) return;
                await UniTask.Yield();
            }

            var table = await GetMatchingTable(tables, key);

            LocalizedString s = new(table, key);

            _stringBinding = new(s, binding);


            s.StringChanged += StringChanged;
        }

        // Tables might not be loaded as well, so we have to await them loading too
        private async UniTask<TableReference> GetMatchingTable(LocalizedStringTable[] tables, string key)
        {
            foreach (var table in tables)
            {
                var handle = table.CurrentLoadingOperationHandle;
                if (!handle.IsValid()) handle = table.GetTableAsync();

                await handle;

                if (table.TableReference.SharedTableData.Contains(key))
                {
                    return table.TableReference;
                }
            }

            throw new BindingException($"No key: {key} found in provided tables.");
        }

        private void StringChanged(string value) => _operation(_element, value);


        public void Dispose()
        {
            _cts.Cancel();
            _cts.Dispose();
            _stringBinding?.Dispose();
        }
    }
}