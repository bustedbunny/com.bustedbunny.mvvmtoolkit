using System;
using System.Collections.Generic;
using System.Reflection;
using MVVMToolkit.Binding;
using UnityEngine.Localization;
using UnityEngine.Localization.Tables;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UIElements;

namespace MVVMToolkit.Localization
{
    public class UILocalization
    {
        private readonly VisualElement _rootVisualElement;
        private readonly LocalizedStringTable _stringTable;
        private StringTable _currentTable;

        private readonly ViewModel _boundObject;
        private readonly Dictionary<LocalizationTextBinding, string> _boundMap = new();

        public UILocalization(LocalizedStringTable stringTable, VisualElement rootVisualElement,
            ViewModel boundObject)
        {
            _boundObject = boundObject;
            _rootVisualElement = rootVisualElement;

            UpdateBindings();

            _stringTable = stringTable;
            _stringTable.TableChanged += OnStringTableChanged;
            OnStringTableChanged();
        }

        public void Dispose()
        {
            if (_stringTable != null)
            {
                _stringTable.TableChanged -= OnStringTableChanged;
            }
        }


        private void UpdateBindings()
        {
            foreach (var (binding, _) in _boundMap)
            {
                binding.Dispose();
            }

            UpdateBindings(_rootVisualElement);
        }

        private void UpdateBindings(VisualElement element)
        {
            var elementHierarchy = element.hierarchy;
            var childCount = elementHierarchy.childCount;
            for (var i = 0; i < childCount; i++)
            {
                if (elementHierarchy[i] is TextElement textElement)
                {
                    if (!GetKey(textElement, out var key))
                    {
                        continue;
                    }

                    var bindableElement = BindTextElement(textElement, _boundObject);
                    _boundMap.Add(bindableElement, key);
                }
            }

            for (var i = 0; i < childCount; i++)
            {
                UpdateBindings(elementHierarchy[i]);
            }
        }

        private static bool GetKey(TextElement element, out string key)
        {
            key = element.text;
            if (string.IsNullOrEmpty(key) || key[0] != '#')
            {
                return false;
            }

            key = key.Substring(1, key.Length - 1);
            return true;
        }

        private static bool TryGetValidProperty(Type type, string name, out PropertyInfo property)
        {
            property = type.GetProperty(name, BindingFlags.Instance | BindingFlags.Public);
            if (property is null || !property.CanRead)
            {
                return false;
            }

            return true;
        }

        private LocalizationTextBinding BindTextElement(TextElement element, ViewModel boundObject)
        {
            // if (element.bindingPath is not null && boundObject is not null)
            // {
            //     // Binding properties are separated by dot
            //     var bindings = element.bindingPath.Split('.');
            //
            //
            //     var boundProperties = new PropertyInfo[bindings.Length];
            //     for (int j = 0; j < bindings.Length; j++)
            //     {
            //         var propertyName = bindings[j];
            //         if (!TryGetValidProperty(boundObject.GetType(), propertyName, out var property))
            //         {
            //             Debug.LogError(
            //                 $"No property \"{propertyName}\" is found on object of type {boundObject.GetType().Name}");
            //             return new BindableTextElement(element);
            //         }
            //
            //         boundProperties[j] = property;
            //     }
            //
            //     var boundText = new BindableTextElement(element, _boundObject, boundProperties);
            //     return boundText;
            // }
            //
            // return new BindableTextElement(element);
            return null;
        }

        private void OnStringTableChanged(StringTable value)
        {
            OnStringTableChanged();
        }

        private void OnStringTableChanged()
        {
            var op = _stringTable.GetTableAsync();
            op.Completed -= OnTableLoaded;
            op.Completed += OnTableLoaded;
        }

        private void OnTableLoaded(AsyncOperationHandle<StringTable> op)
        {
            _currentTable = op.Result;
            LocalizeAll();
        }

        /// <summary>
        /// Change the current key of localization
        /// </summary>
        /// <param name="textElement"></param>
        /// <param name="newKey">New localization key</param>
        public void Rebind(TextElement textElement, string newKey)
        {
            foreach (var (bind, _) in _boundMap)
            {
                if (textElement == bind.TextElement)
                {
                    _boundMap[bind] = newKey;
                    Localize(bind, newKey);
                    _rootVisualElement.MarkDirtyRepaint();
                    return;
                }
            }
        }

        private void Localize(LocalizationTextBinding bind, string key)
        {
        }

        private void LocalizeAll()
        {
            foreach (var (bind, key) in _boundMap)
            {
                Localize(bind, key);
            }

            _rootVisualElement.MarkDirtyRepaint();
        }
    }
}