using System;
using System.ComponentModel;
using System.Reflection;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UIElements;
using Debug = System.Diagnostics.Debug;

namespace MVVMToolkit.Binding
{
    public class ValueChangedStore : BindingPathStore<ValueChangedBinding>
    {
        public override char Symbol() => '0';

        public ValueChangedStore(object binding) : base(binding)
        {
        }

        public override void Process(VisualElement element, string key)
        {
            try
            {
                var type = element.GetType();
                foreach (var impl in type.GetInterfaces())
                {
                    if (!impl.IsGenericType) continue;
                    if (impl.GetGenericTypeDefinition() == typeof(INotifyValueChanged<>))
                    {
                        var targetType = impl.GetGenericArguments()[0];
                        boundingMap.Add(CreateSupportedType(targetType, element, key), key);
                    }
                }
            }
            catch (Exception e)
            {
                UnityEngine.Debug.LogException(e);
            }
        }

        private ValueChangedBinding CreateSupportedType(Type type, VisualElement element, string key)
        {
            if (type == typeof(string))
                return new ValueChangedBinding<string>((INotifyValueChanged<string>)element, bindingContext, key);

            if (type == typeof(int))
                return new ValueChangedBinding<int>((INotifyValueChanged<int>)element, bindingContext, key);

            if (type == typeof(float))
                return new ValueChangedBinding<float>((INotifyValueChanged<float>)element, bindingContext, key);

            if (type == typeof(double))
                return new ValueChangedBinding<double>((INotifyValueChanged<double>)element, bindingContext, key);

            if (type == typeof(Vector2))
                return new ValueChangedBinding<Vector2>((INotifyValueChanged<Vector2>)element, bindingContext, key);

            if (type == typeof(Vector3))
                return new ValueChangedBinding<Vector3>((INotifyValueChanged<Vector3>)element, bindingContext, key);

            if (type == typeof(Vector4))
                return new ValueChangedBinding<Vector4>((INotifyValueChanged<Vector4>)element, bindingContext, key);

            if (type == typeof(int2))
                return new ValueChangedBinding<int2>((INotifyValueChanged<int2>)element, bindingContext, key);

            if (type == typeof(int3))
                return new ValueChangedBinding<int3>((INotifyValueChanged<int3>)element, bindingContext, key);

            if (type == typeof(int4))
                return new ValueChangedBinding<int4>((INotifyValueChanged<int4>)element, bindingContext, key);

            throw new InvalidOperationException($"Tried to parse type {type.Name} which is not supported.");
        }
    }

    public class ValueChangedBinding<T> : ValueChangedBinding
    {
        private readonly INotifyValueChanged<T> _element;
        private readonly INotifyPropertyChanged _boundObject;
        private readonly Action<T> _setter;
        private readonly Func<T> _getter;

        private readonly string _key;

        public ValueChangedBinding(INotifyValueChanged<T> element, object boundObject, string key)
        {
            BindingUtility.GetTargetObject(boundObject, key, out var target, out var propertyName);

            var bindingType = target.GetType();
            var property = bindingType.GetProperty(propertyName,
                BindingFlags.Public | BindingFlags.Instance | BindingFlags.SetProperty);

            Debug.Assert(property != null, nameof(property) + " != null");
            _setter = (Action<T>)property.GetSetMethod().CreateDelegate(typeof(Action<T>), target);

            _getter = (Func<T>)property.GetGetMethod().CreateDelegate(typeof(Func<T>), target);
            _key = key;
            _element = element;
            _boundObject = (INotifyPropertyChanged)target;
            _boundObject.PropertyChanged += OnBoundObjectOnPropertyChanged;
            _element.RegisterValueChangedCallback(OnValueChanged);

            var get = _getter();
            _element.value = get;
        }

        private void OnBoundObjectOnPropertyChanged(object _, PropertyChangedEventArgs args)
        {
            if (args.PropertyName == _key)
            {
                _element.value = _getter();
            }
        }


        private void OnValueChanged(ChangeEvent<T> evt) => _setter(evt.newValue);


        public override void Dispose()
        {
            _boundObject.PropertyChanged -= OnBoundObjectOnPropertyChanged;
            _element.UnregisterValueChangedCallback(OnValueChanged);
        }
    }

    public abstract class ValueChangedBinding : IBindable
    {
        public abstract void Dispose();
    }
}