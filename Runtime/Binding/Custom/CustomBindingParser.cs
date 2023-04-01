using System.ComponentModel;
using UnityEngine.UIElements;

namespace MVVMToolkit.Binding.Custom
{
    public class CustomBindingParser : BindingParser<ICustomBinder>
    {
        public CustomBindingParser(INotifyPropertyChanged binding) : base(binding) { }

        public override void Process(VisualElement element, string key)
        {
            var binder = CustomBinderMap.GetInstance(key);
            binder.Bind(element, bindingContext);
            boundingMap.Add(binder, key);
        }

        public override char Symbol() => '*';
    }
}