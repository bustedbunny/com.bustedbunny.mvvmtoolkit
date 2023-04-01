using System.Collections;
using System.ComponentModel;
using MVVMToolkit.Binding.CollectionBinding.Generics;
using MVVMToolkit.Binding.CollectionBinding.Utility;
using UnityEngine.UIElements;

namespace MVVMToolkit.Binding.CollectionBinding
{
    public class CollectionParser : BindingParser<ICollectionBinder>
    {
        public CollectionParser(INotifyPropertyChanged binding) : base(binding) { }

        public override void Process(VisualElement element, string key)
        {
            var keys = key.Split(':');
            var left = keys[0];
            var right = keys[1];
            var template = element.parent.Q<DataTemplate>(right);

            var binder = CollectionBinderMap.GetBinder(element.GetType());
            ParsingUtility.GetTargetObject(bindingContext, left, out var target, out var propertyName);
            var data = PropertyUtility.GetGetProperty(target, propertyName).GetValue(target);

            binder.Bind(element, (IList)data, template);

            boundingMap.Add(binder, key);
        }

        public override char Symbol() => '~';
    }
}