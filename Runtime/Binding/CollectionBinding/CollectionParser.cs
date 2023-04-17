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
            var parent = element.parent;
            var indexInParent = parent.IndexOf(element);
            if (parent.childCount <= indexInParent || parent[indexInParent + 1] is not DataTemplate template)
            {
                throw new BindingException("No data template was found");
            }

            var keys = key.Split(':');
            var left = keys[0];

            var binder = CollectionBinderMap.GetBinder(element.GetType());
            ParsingUtility.GetTargetObject(bindingContext, left, out var target, out var propertyName);
            var data = PropertyUtility.GetGetProperty(target, propertyName).GetValue(target);

            var commandPath = keys.Length > 1 ? keys[1] : null;
            var command = commandPath is null ? null : CommandUtility.GetCommand(bindingContext, commandPath);
            binder.Bind(element, (IList)data, template, command);

            boundingMap.Add(binder, key);
        }

        public override char Symbol() => '~';
    }
}