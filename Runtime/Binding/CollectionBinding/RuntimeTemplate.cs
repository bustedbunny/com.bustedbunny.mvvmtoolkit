using System.Collections.Generic;
using MVVMToolkit.Binding.CollectionBinding.Generics;
using UnityEngine.UIElements;

namespace MVVMToolkit.Binding.CollectionBinding
{
    public class RuntimeTemplate : VisualElement
    {
        public readonly List<(VisualElement, IItemCollectionBinder)> bindings = new();

        public void AddBinding(VisualElement element, IItemCollectionBinder binder)
        {
            bindings.Add((element, binder));
        }
    }
}