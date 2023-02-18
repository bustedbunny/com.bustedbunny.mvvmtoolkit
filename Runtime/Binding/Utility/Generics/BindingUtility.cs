using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using MVVMToolkit.Binding.Localization;
using MVVMToolkit.Binding.Localization.Source;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

namespace MVVMToolkit.Binding.Generics
{
    public static class BindingUtility
    {
        private static readonly Dictionary<(Type, Type), IMultiSolver> MultiMap = new();
        private static readonly Dictionary<Type, ISingleSolver> SingleMap = new();

        static BindingUtility()
        {
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (var type in assembly.GetTypes())
                {
                    if (!type.IsAbstract && typeof(ISingleSolver).IsAssignableFrom(type))
                    {
                        var solver = (ISingleSolver)Activator.CreateInstance(type);
                        SingleMap.Add(solver.Type, solver);
                    }
                    else if (!type.IsAbstract && typeof(IMultiSolver).IsAssignableFrom(type))
                    {
                        var solver = (IMultiSolver)Activator.CreateInstance(type);
                        MultiMap.Add((solver.GetterType, solver.SetterType), solver);
                    }
                }
            }
        }


        public static Action SetAction(PropertyInfo get, object source, PropertyInfo set, object target)
        {
            var getMethod = get.GetGetMethod(true);
            var setMethod = set.GetSetMethod(true);

            var getType = get.PropertyType;
            var setType = set.PropertyType;

            if (getType == setType)
            {
                if (SingleMap.TryGetValue(getType, out var solver))
                {
                    return solver.Solve(getMethod, source, setMethod, target);
                }

                WarnNoSolverSingle(getType);
            }
            else
            {
                if (MultiMap.TryGetValue((getType, setType), out var solver))
                {
                    return solver.Solve(get.GetGetMethod(true), source, set.GetSetMethod(true), target);
                }

                WarnNoSolverMulti(getType, setType);
            }

            // Reflection fallback
            return () => { set.SetValue(target, get.GetValue(source)); };
        }

        public static ILocalizationVariable LocalizationVariable(PropertyInfo property, object binding)
        {
            var getMethod = property.GetGetMethod(true);

            if (SingleMap.TryGetValue(getMethod.ReturnType, out var solver))
            {
                return solver.SolveLocalization(getMethod, binding);
            }

            // Reflection fallback
            WarnNoSolverSingle(property.PropertyType);
            return new LocalizationVariable<string>(() => property.GetValue(binding).ToString());
        }

        public static ValueChangedBinding ValueChangedBinding(Type valueChangedType, VisualElement element, string key,
            INotifyPropertyChanged binding)
        {
            ParsingUtility.GetTargetObject(binding, key, out var target, out var propertyName);
            var property = PropertyUtility.GetGetSetProperty(target, propertyName);

            if (valueChangedType == property.PropertyType)
            {
                if (SingleMap.TryGetValue(valueChangedType, out var solver))
                {
                    return solver.SolveValueChanged(element, binding, property);
                }

                WarnNoSolverSingle(valueChangedType);
            }
            else
            {
                throw new BindingException("ValueChangedBinding types do not match: " +
                                           $"{property.PropertyType.Name} {valueChangedType.Name}.");
            }

            throw new BindingException(
                $"No IGenericsSolver found for type {valueChangedType.Name}. ValueChanged binding must have one.");
        }

        public static Action StringFormatSetAction(object binding, string format, object[] array, int index,
            out string propertyName, out object target)
        {
            ParsingUtility.GetTargetObject(binding, format, out target, out propertyName);

            var property = PropertyUtility.GetGetProperty(target, propertyName);

            if (SingleMap.TryGetValue(property.PropertyType, out var solver))
            {
                return solver.SolveArraySetElement(property, target, array, index);
            }

            var targetCopy = target;

            // Reflection fallback
            WarnNoSolverSingle(property.PropertyType);
            return () => array[index] = property.GetValue(targetCopy);
        }

        public static LocalizedAssetBinding AssetBinding(PropertyInfo setProp, object target, LocalizedAssetTable table,
            string key, Type assetType)
        {
            var setterType = setProp.PropertyType;

            Action<Object> assetSetter;

            if (MultiMap.TryGetValue((assetType, setterType), out var solver))
            {
                assetSetter = solver.ResolveAssetSetter(setProp, target);
                return new(table, key, assetSetter);
            }

            WarnNoSolverMulti(assetType, setterType);
            assetSetter = o => setProp.SetValue(target, o);
            return new(table, key, assetSetter);
        }

        private static void WarnNoSolverSingle(Type type)
        {
            Debug.LogWarning($"A reflection fallback with type {type.Name} was used." +
                             $" Implement {typeof(SingleSolver<>).Name}<{type.Name}>.");
        }

        private static void WarnNoSolverMulti(Type get, Type set)
        {
            Debug.LogWarning($"A reflection fallback for type pair {get.Name}->{set.Name} was used." +
                             $" Implement {typeof(MultiSolver<,>).Name}<{get.Name}, {set.Name}>.");
        }
    }
}