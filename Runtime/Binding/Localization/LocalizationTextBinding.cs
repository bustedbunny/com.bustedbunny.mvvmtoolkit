using System;
using System.ComponentModel;
using System.Reflection;
using UnityEngine.Localization;
using UnityEngine.Localization.SmartFormat.PersistentVariables;
using UnityEngine.UIElements;

namespace MVVMToolkit.Binding.Localization
{
    public class LocalizationTextBinding : IBindable
    {
        private readonly LocalizedString _string;
        private readonly string _tableCollectionName;
        private readonly LocalizedStringTable _table;
        private readonly string _key;
        private readonly object _binding;
        private readonly TextElement _element;

        private (INotifyPropertyChanged, PropertyChangedEventHandler)[] _bindings;

        public LocalizationTextBinding(TextElement element, object binding, string key, LocalizedStringTable table)
        {
            _element = element;
            _binding = binding;
            _key = key;
            _table = table;
            _tableCollectionName = table.TableReference.TableCollectionName;
            TablePostprocessor.OnTableLoaded += OnTableLoaded;

            _string = new LocalizedString(table.TableReference, key);
            _string.StringChanged += StringChanged;
        }

        private void StringChanged(string value) => _element.text = value;

        private static readonly char[] Chars = { '.', ':' };

        private void OnTableLoaded(string tableCollectionName)
        {
            if (_tableCollectionName != tableCollectionName) return;
            var table = _table.GetTableAsync().Result;
            var entry = table[_key];
            if (entry is null || !entry.IsSmart) return;

            var formats = BindingUtility.GetFormatKeys(entry.Value);
            if (formats is not null)
            {
                CleanUpBindings();
                _bindings = new (INotifyPropertyChanged, PropertyChangedEventHandler)[formats.Length];
                for (int i = 0; i < formats.Length; i++)
                {
                    var format = formats[i];
                    var ind = format.IndexOfAny(Chars);
                    if (ind != -1)
                    {
                        format = format[..ind];
                    }

                    var (property, targetObject) = GetBinding(_binding, format);

                    var stringVar = new StringVariable
                    {
                        Value = property.GetValue(targetObject).ToString()
                    };

                    var name = property.Name;

                    var action = new PropertyChangedEventHandler((_, args) =>
                    {
                        if (args.PropertyName != name) return;
                        stringVar.Value = property.GetValue(targetObject).ToString();
                    });

                    targetObject.PropertyChanged += action;

                    _bindings[i] = (targetObject, action);
                    _string.Add(format, stringVar);
                }
            }
        }

        private static (PropertyInfo, INotifyPropertyChanged) GetBinding(object bindingObject, string format)
        {
            BindingUtility.GetTargetObject(bindingObject, format, out var target, out var propertyName);

            if (!TryGetValidProperty(target.GetType(), propertyName, out var property))
            {
                throw new NullReferenceException(
                    $"No property \"{propertyName}\" is found on object of type {target.GetType().Name}");
            }

            return (property, (INotifyPropertyChanged)target);
        }

        private static bool TryGetValidProperty(Type type, string name, out PropertyInfo property)
        {
            property = type.GetProperty(name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.GetProperty);
            return property is not null;
        }

        private void CleanUpBindings()
        {
            if (_string.Count > 0)
                _string.Clear();
            if (_bindings is null) return;
            foreach (var (boundObject, action) in _bindings)
            {
                boundObject.PropertyChanged -= action;
            }

            _bindings = null;
        }

        public void Dispose()
        {
            TablePostprocessor.OnTableLoaded -= OnTableLoaded;
            CleanUpBindings();
        }
    }
}