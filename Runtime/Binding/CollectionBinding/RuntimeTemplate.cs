using System;
using UnityEngine.UIElements;

namespace MVVMToolkit.Binding.CollectionBinding
{
    public class RuntimeTemplate
    {
        public readonly int parentKey;
        public readonly Func<VisualElement> element;

        public RuntimeTemplate(int parentKey, Func<VisualElement> element)
        {
            this.parentKey = parentKey;
            this.element = element;
        }
    }
}