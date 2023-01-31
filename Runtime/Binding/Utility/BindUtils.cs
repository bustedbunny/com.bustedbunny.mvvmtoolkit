using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using MVVMToolkit.Binding.Generics;
using MVVMToolkit.Binding.Localization.Source;
using UnityEngine;
using UnityEngine.UIElements;

namespace MVVMToolkit.Binding
{
    public static class BindUtils
    {
        static BindUtils()
        {
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (var type in assembly.GetTypes())
                {
                    if (!type.IsAbstract && typeof(IGenericsSolver).IsAssignableFrom(type))
                    {
                        var solver = (IGenericsSolver)Activator.CreateInstance(type);
                        SolverMap.Add(solver.Type, solver);
                    }
                }
            }
        }

        private static readonly Dictionary<Type, IGenericsSolver> SolverMap = new();

        public static Action SetAction(PropertyInfo get, object source, PropertyInfo set, object target)
        {
            var type = get.PropertyType;
            if (type != set.PropertyType)
                throw new BindingException($"Get type: {type.Name} is not equal to Set type: {set.PropertyType.Name}.");
            var getMethod = get.GetGetMethod();
            var setMethod = set.GetSetMethod();


            if (SolverMap.TryGetValue(type, out var solver))
            {
                return solver.Solve(getMethod, source, setMethod, target);
            }

            // Reflection fallback
            Debug.LogWarning($"Had to do a reflection fallback with type {type.Name}");
            return () => { set.SetValue(target, get.GetValue(source)); };
        }

        public static ILocalizationVariable LocalizationVariable(PropertyInfo property, object binding)
        {
            var getMethod = property.GetGetMethod();

            if (SolverMap.TryGetValue(getMethod.ReturnType, out var solver))
            {
                return solver.SolveLocalization(getMethod, binding);
            }

            // Reflection fallback
            Debug.LogWarning($"Had to do a reflection fallback with type {property.PropertyType.Name}");
            return new LocalizationVariable<string>(() => property.GetValue(binding).ToString());
        }

        public static ValueChangedBinding ValueChangedBinding(Type type, VisualElement element, string key,
            INotifyPropertyChanged binding)
        {
            if (SolverMap.TryGetValue(type, out var solver))
            {
                return solver.SolveValueChanged(type, element, key, binding);
            }

            throw new BindingException($"No IGenericsSolver found for type {type.Name}.");
        }

        public static bool TryGetPropertyGet(object source, string propertyName, out PropertyInfo property)
        {
            const BindingFlags flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.GetProperty;
            property = source.GetType().GetProperty(propertyName, flags);
            return property is not null;
        }
    }
}