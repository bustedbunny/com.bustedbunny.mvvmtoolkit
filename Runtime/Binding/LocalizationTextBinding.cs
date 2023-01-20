using System;
using System.ComponentModel;
using System.Reflection;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Tables;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UIElements;

namespace MVVMToolkit.Binding
{
    public class LocalizationStore : TextBindingStore<LocalizationTextBinding>
    {
        private readonly LocalizedStringTable _stringTable;
        private StringTable _currentTable;
        public override char Symbol() => '#';

        public LocalizationStore(object viewModel, LocalizedStringTable stringTable) : base(viewModel)
        {
            _stringTable = stringTable;
            _stringTable.TableChanged += TableChanged;
        }

        public override void PostBindingCallback()
        {
            TableChanged(null);
        }


        public override void Dispose()
        {
            base.Dispose();
            _stringTable.TableChanged -= TableChanged;
        }

        private void TableChanged(StringTable value)
        {
            var op = _stringTable.GetTableAsync();
            op.Completed -= OnTableLoaded;
            op.Completed += OnTableLoaded;
        }

        private void OnTableLoaded(AsyncOperationHandle<StringTable> obj)
        {
            _currentTable = obj.Result;
            LocalizeAll();
        }

        private void LocalizeAll()
        {
            foreach (var (bind, _) in boundingMap)
            {
                bind.Localize(_currentTable);
            }
        }

        public override void Process(VisualElement element, string key)
        {
            try
            {
                boundingMap.Add(new LocalizationTextBinding((TextElement)element, bindingContext, key), key);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }
    }

    public class LocalizationTextBinding : IBindable
    {
        private readonly object[] _return;

        private readonly object _boundObject;
        private StringTableEntry _currentEntry;

        private readonly string _mainKey;

        public void Localize(StringTable table)
        {
            var entry = table[_mainKey];
            if (entry == null)
            {
                Debug.LogWarning($"No translation in {table.LocaleIdentifier} for key: {_mainKey}");
                TextElement.text = $"#{_mainKey}";
            }
            else
            {
                if (_currentEntry == entry) return;
                _currentEntry = entry;
                UpdateText();
            }
        }

        public TextElement TextElement { get; }

        private (INotifyPropertyChanged, PropertyChangedEventHandler)[] _bindings;

        public LocalizationTextBinding(TextElement text, object boundObject, string key)
        {
            _boundObject = boundObject;
            TextElement = text;


            var bindings = key.Split(':');

            _mainKey = bindings[0];

            if (bindings.Length > 1)
            {
                var binds = new (PropertyInfo, INotifyPropertyChanged)[bindings.Length - 1];
                for (int i = 1; i < bindings.Length; i++)
                {
                    BindingUtility.GetTargetObject(boundObject, bindings[i], out var target, out var propertyName);

                    if (!TryGetValidProperty(target.GetType(), propertyName, out var property))
                    {
                        throw new NullReferenceException(
                            $"No property \"{propertyName}\" is found on object of type {target.GetType().Name}");
                    }

                    binds[i - 1] = (property, (INotifyPropertyChanged)target);
                }

                _return = new object[binds.Length];
                _bindings = new (INotifyPropertyChanged, PropertyChangedEventHandler)[binds.Length];

                for (int i = 0; i < binds.Length; i++)
                {
                    var bind = binds[i];
                    var property = bind.Item1;
                    var targetObject = bind.Item2;
                    // Assign value if it was assigned before
                    _return[i] = property.GetValue(targetObject).ToString();

                    var index = i;
                    var name = bind.Item1.Name;

                    var action = new PropertyChangedEventHandler((_, args) =>
                    {
                        if (args.PropertyName != name) return;
                        _return[index] = property.GetValue(targetObject).ToString();
                        UpdateText();
                    });
                    _bindings[i] = (targetObject, action);
                    targetObject.PropertyChanged += action;
                }
            }
        }

        private static bool TryGetValidProperty(Type type, string name, out PropertyInfo property)
        {
            property = type.GetProperty(name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.GetProperty);
            return property is not null;
        }

        public void Dispose()
        {
            if (_bindings is null) return;
            foreach (var (obj, action) in _bindings)
            {
                if (obj is not null)
                {
                    obj.PropertyChanged -= action;
                }
            }
        }


        private void UpdateText()
        {
            if (_currentEntry is not null)
                TextElement.text = _currentEntry.GetLocalizedString(_return);
        }
    }
}