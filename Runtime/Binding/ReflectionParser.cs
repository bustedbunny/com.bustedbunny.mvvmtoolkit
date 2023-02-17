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

        private static void GetTargetProperty(object root, string key, out object target, out PropertyInfo property)
        {
            var paths = key.Split('.');

            target = root;
            var rootType = root.GetType();
            for (int i = 0; i < paths.Length - 1; i++)
            {
                var path = paths[i];
                property = rootType.GetProperty(path, BindingFlags.Instance | BindingFlags.Public);
                rootType = property.PropertyType;
                target = property.GetValue(target);
            }

            var lenght = paths.Length;
            property = rootType.GetProperty(paths[lenght - 1], BindingFlags.Instance | BindingFlags.Public);
            if (property is null)
                throw new BindingException($"Type {rootType.Name} has no property of name {paths[lenght - 1]}.");
        }

        public ReflectionBinding(VisualElement element, string key, INotifyPropertyChanged binding)
        {
            _binding = binding;

            var separator = key.IndexOf('=');
            var left = key[..separator];
            separator++;
            var right = key[separator..];

            GetTargetProperty(element, left, out var setTarget, out var setProp);
            GetTargetProperty(binding, right, out var getTarget, out var getProp);

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


        public void Dispose()
        {
            if (_binding is not null)
            {
                _binding.PropertyChanged -= _action;
            }
        }
    }
}