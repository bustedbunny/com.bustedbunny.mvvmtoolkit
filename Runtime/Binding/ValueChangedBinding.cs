using System;
using System.ComponentModel;
using MVVMToolkit.Binding.Generics;
using UnityEngine.UIElements;

namespace MVVMToolkit.Binding
{
    public class ValueChangedParser : BindingParser<ValueChangedBinding>
    {
        public override char Symbol() => '%';

        public ValueChangedParser(INotifyPropertyChanged binding) : base(binding) { }

        public override void Process(VisualElement element, string key)
        {
            var type = element.GetType();
            foreach (var impl in type.GetInterfaces())
            {
                if (!impl.IsGenericType) continue;
                if (impl.GetGenericTypeDefinition() == typeof(INotifyValueChanged<>))
                {
                    var targetType = impl.GetGenericArguments()[0];
                    var binding = BindingUtility.ValueChangedBinding(targetType, element, key, bindingContext);
                    boundingMap.Add(binding, key);
                    return;
                }
            }
        }
    }

    public class ValueChangedBinding<TValue> : ValueChangedBinding
    {
        private readonly INotifyValueChanged<TValue> _element;
        private readonly INotifyPropertyChanged _boundObject;
        private readonly Action<TValue> _setter;
        private readonly Func<TValue> _getter;

        private readonly string _key;

        public ValueChangedBinding(INotifyValueChanged<TValue> element, INotifyPropertyChanged root, Action<TValue> set,
            Func<TValue> get, string propertyName)
        {
            _key = propertyName;
            _element = element;
            _boundObject = root;
            _boundObject.PropertyChanged += OnBoundObjectOnPropertyChanged;
            _element.RegisterValueChangedCallback(OnValueChanged);

            _setter = set;
            _getter = get;

            _element.value = get();
        }

        private void OnBoundObjectOnPropertyChanged(object _, PropertyChangedEventArgs args)
        {
            if (args.PropertyName == _key) _element.value = _getter();
        }

        private void OnValueChanged(ChangeEvent<TValue> evt) => _setter(evt.newValue);


        public override void Unbind()
        {
            _boundObject.PropertyChanged -= OnBoundObjectOnPropertyChanged;
            _element.UnregisterValueChangedCallback(OnValueChanged);
        }
    }

    public abstract class ValueChangedBinding : IElementBinding
    {
        public abstract void Unbind();
    }
}