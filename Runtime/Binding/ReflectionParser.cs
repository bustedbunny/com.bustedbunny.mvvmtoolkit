using System;
using System.ComponentModel;
using System.Reflection;
using MVVMToolkit.Binding.Generics;
using UnityEngine.Assertions;
using UnityEngine.UIElements;

namespace MVVMToolkit.Binding
{
    public class ReflectionParser : BindingParser<ReflectionBinding>
    {
        private readonly INotifyPropertyChanged _binding;

        public ReflectionParser(INotifyPropertyChanged binding) : base(binding)
        {
            _binding = binding;
        }

        public override char Symbol() => '^';

        public override void Process(VisualElement element, string key)
        {
            boundingMap.Add(new(element, key, _binding), key);
        }
    }

    public class ReflectionBinding : IElementBinding
    {
        private readonly PropertyChangedEventHandler _action;
        private readonly INotifyPropertyChanged _binding;

        public ReflectionBinding(VisualElement element, string key, INotifyPropertyChanged binding)
        {
            _binding = binding;

            ResolveBinding(element, _binding, key,
                out var getTarget, out var setTarget,
                out var getProp, out var setProp);

            Assert.AreEqual(setProp.PropertyType, getProp.PropertyType);

            var propertyName = getProp.Name;


            var setAction = BindingUtility.SetAction(getProp, getTarget, setProp, setTarget);

            _action = (_, args) =>
            {
                if (args.PropertyName == propertyName)
                {
                    setAction();
                }
            };

            setAction();

            _binding.PropertyChanged += _action;
        }


        public void Unbind()
        {
            if (_binding is not null)
            {
                _binding.PropertyChanged -= _action;
            }
        }

        private static void ResolveBinding(object element, object binding, string key,
            out object getTarget, out object setTarget,
            out PropertyInfo getProp, out PropertyInfo setProp)
        {
            var separator = key.IndexOf('=');
            var left = key[..separator];
            separator++;
            var right = key[separator..];

            ParsingUtility.GetTargetObject(element, left, out setTarget, out var setPropName);
            ParsingUtility.GetTargetObject(binding, right, out getTarget, out var getPropName);

            setProp = PropertyUtility.GetSetProperty(setTarget, setPropName);
            getProp = PropertyUtility.GetGetProperty(getTarget, getPropName);
        }
    }
}