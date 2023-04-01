using System;
using System.Collections.Generic;
using System.Linq;
using MVVMToolkit.Binding;
using MVVMToolkit.Binding.CollectionBinding;
using MVVMToolkit.Binding.CollectionBinding.Generics;
using MVVMToolkit.Binding.CollectionBinding.Utility;
using MVVMToolkit.Binding.Generics;
using UnityEngine.UIElements;

// ReSharper disable once CheckNamespace
namespace MVVMToolkit
{
    public class DataTemplate : VisualElement
    {
        public DataTemplate()
        {
            style.display = new(DisplayStyle.None);
        }

        private List<ElementData> _list;

        public VisualElement Instantiate()
        {
            if (_list is null)
            {
                Init();
            }

            var root = new RuntimeTemplate();
            var list = new List<VisualElement>();

            foreach (var template in _list)
            {
                var element = template.element();
                list.Add(element);
                if (template.parentKey == -1)
                {
                    root.Add(element);
                }
                else
                {
                    list.ElementAt(template.parentKey).Add(element);
                }

                if (_binderMap.TryGetValue(list.Count - 1, out var binder))
                {
                    root.AddBinding(element, binder);
                }
            }


            return root;
        }

        private class ElementData
        {
            public readonly int parentKey;
            public readonly Func<VisualElement> element;

            public ElementData(int parentKey, Func<VisualElement> element)
            {
                this.parentKey = parentKey;
                this.element = element;
            }
        }

        public IReadOnlyDictionary<int, IItemCollectionBinder> Bindigns => _binderMap;

        private Dictionary<int, IItemCollectionBinder> _binderMap;

        private void Init()
        {
            var sourceList = new List<VisualElement>();
            _binderMap = new();
            _list = new();
            this.Query(className: "template").ForEach(x =>
            {
                var ctor = TemplateReflectionUtils.GetConstructor(x);
                var ind = sourceList.Count;
                sourceList.Add(x);
                var parentInd = -1;
                if (x.parent != this)
                {
                    parentInd = sourceList.IndexOf(x.parent);
                }

                var keys = ParsingUtility.GetFormatKeys(x.viewDataKey);
                if (keys is not null)
                {
                    foreach (var key in keys)
                    {
                        if (key.StartsWith(':'))
                        {
                            _binderMap.Add(ind, ItemCollectionMap.GetInstance(key[1..]));
                        }
                    }
                }

                _list.Add(new(parentInd, ctor));
            });
            Clear();
        }

        public new class UxmlFactory : UxmlFactory<DataTemplate, UxmlTraits> { }

        public new class UxmlTraits : VisualElement.UxmlTraits { }
    }
}