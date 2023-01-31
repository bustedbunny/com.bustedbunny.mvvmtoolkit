using System;
using System.ComponentModel;
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
                    var binding = BindUtils.ValueChangedBinding(targetType, element, key, bindingContext);
                    boundingMap.Add(binding, key);
                    return;
                }
            }
        }
    }

    public class ValueChangedBinding<T> : ValueChangedBinding
    {
        private readonly INotifyValueChanged<T> _element;
        private readonly INotifyPropertyChanged _boundObject;
        private readonly Action<T> _setter;
        private readonly Func<T> _getter;

        private readonly string _key;

        public ValueChangedBinding(INotifyValueChanged<T> element, INotifyPropertyChanged root, Action<T> set,
            Func<T> get, string propertyName)
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

        private void OnValueChanged(ChangeEvent<T> evt) => _setter(evt.newValue);


        public override void Dispose()
        {
            _boundObject.PropertyChanged -= OnBoundObjectOnPropertyChanged;
            _element.UnregisterValueChangedCallback(OnValueChanged);
        }
    }

    public abstract class ValueChangedBinding : IElementBinding
    {
        public abstract void Dispose();
    }
}