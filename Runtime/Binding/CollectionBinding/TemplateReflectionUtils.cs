using System;
using System.Diagnostics;
using System.Linq;
using UnityEngine.UIElements;

namespace MVVMToolkit.Binding.CollectionBinding
{
    public static class TemplateReflectionUtils
    {
        public static Func<VisualElement> GetConstructor(VisualElement source)
        {
            var type = source.GetType();
            var ctor = type.GetConstructor(Type.EmptyTypes);
            Debug.Assert(ctor != null, nameof(ctor) + " != null");

            var pm = source.pickingMode;
            var name = source.name;
            var focusable = source.focusable;
            var tabIndex = source.tabIndex;

            var classes = source.GetClasses().ToArray();
            return () =>
            {
                var result = (VisualElement)ctor.Invoke(null);
                result.pickingMode = pm;
                result.name = name;
                result.focusable = focusable;
                result.tabIndex = tabIndex;
                foreach (var className in classes)
                {
                    result.AddToClassList(className);
                }

                return result;
            };
        }
    }
}