using System.ComponentModel;
using System.Reflection;
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
            boundingMap.Add(new ReflectionBinding(element, key, _binding), key);
        }
    }

    public class ReflectionBinding : IElementBinding
    {
        private readonly PropertyChangedEventHandler _action;
        private readonly INotifyPropertyChanged _binding;

        public ReflectionBinding(VisualElement element, string key, INotifyPropertyChanged binding)
        {
            _binding = binding;

            var separator = key.IndexOf('=');
            var left = key[..separator];
            separator++;
            var right = key[separator..];

            BindingUtility.GetTargetObject(element, left, out var setTarget, out var setPropName);
            BindingUtility.GetTargetObject(binding, right, out var getTarget, out var getPropName);

            var setProp = PropertyUtility.GetSetProperty(setTarget, setPropName);
            var getProp = PropertyUtility.GetGetProperty(getTarget, getPropName);

            Assert.AreEqual(setProp.PropertyType, getProp.PropertyType);
            var propertyName = getProp.Name;


            var setAction = GenericsUtility.SetAction(getProp, getTarget, setProp, setTarget);

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
    }
}