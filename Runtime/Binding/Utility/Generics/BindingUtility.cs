using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
using MVVMToolkit.Binding.Localization;
using MVVMToolkit.Binding.Localization.Source;
using MVVMToolkit.TypeSerialization;
using UnityEngine.Localization;
using UnityEngine.UIElements;
using Debug = UnityEngine.Debug;
using Object = UnityEngine.Object;

namespace MVVMToolkit.Binding.Generics
{
    public static class BindingUtility
    {
        private static readonly Dictionary<(Type, Type), IMultiSolver> MultiMap;
        private static readonly Dictionary<Type, ISingleSolver> SingleMap;

        static BindingUtility()
        {
            SingleMap = new();
            MultiMap = new();

            foreach (var type in TypeUtility.GetTypes(typeof(ISingleSolver)))
            {
                var solver = (ISingleSolver)Activator.CreateInstance(type);
                SingleMap.Add(solver.Type, solver);
            }

            foreach (var type in TypeUtility.GetTypes(typeof(IMultiSolver)))
            {
                var solver = (IMultiSolver)Activator.CreateInstance(type);
                MultiMap.Add((solver.GetterType, solver.SetterType), solver);
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

            // Generic fallback
            var fallback = FallbackUtility.CreateSolver(source.GetType(), getType, target.GetType(), setType);
            return fallback.Solve(getMethod, source, setMethod, target);
        }

        public static ILocalizationVariable LocalizationVariable(PropertyInfo property, object binding)
        {
            var getMethod = property.GetGetMethod(true);

            if (SingleMap.TryGetValue(getMethod.ReturnType, out var solver))
            {
                return solver.SolveLocalization(getMethod, binding);
            }

            // Generic fallback
            WarnNoSolverSingle(property.PropertyType);
            var localizationType = typeof(LocalizationVariable<>).MakeGenericType(getMethod.ReturnType);
            var delegateType = typeof(Func<>).MakeGenericType(getMethod.ReturnType);
            var del = getMethod.CreateDelegate(delegateType, binding);
            return (ILocalizationVariable)Activator.CreateInstance(localizationType, del);
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
            }
            else
            {
                throw new BindingException("ValueChangedBinding types do not match: " +
                                           $"{property.PropertyType.Name} {valueChangedType.Name}.");
            }

            // Generic fallback
            WarnNoSolverSingle(valueChangedType);
            var genericType = typeof(ValueChangedFallback<>).MakeGenericType(valueChangedType);
            var fallback = (IValueChangedFallback)Activator.CreateInstance(genericType);
            return fallback.SolveValueChanged(element, binding, property);
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

            // Generic fallback
            WarnNoSolverSingle(property.PropertyType);

            var type = typeof(StringFormatFallback<>).MakeGenericType(property.PropertyType);
            var fallback = (IStringFormatFallback)Activator.CreateInstance(type);
            return fallback.SolveArraySetElement(property, target, array, index);
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

            // Generic fallback
            WarnNoSolverMulti(assetType, setterType);

            var type = typeof(AssetSetterFallback<>).MakeGenericType(setterType);
            var fallback = (IAssetSetterFallback)Activator.CreateInstance(type);
            assetSetter = fallback.ResolveAssetSetter(setProp, target);
            return new(table, key, assetSetter);
        }

        [Conditional("MVVMTK_FALLBACK_WARNINGS")]
        private static void WarnNoSolverSingle(Type type)
        {
            Debug.LogWarning($"A generic fallback with type {type.Name} was used." +
                             $" Implement {typeof(SingleSolver<>).Name}<{type.Name}>.");
        }

        [Conditional("MVVMTK_FALLBACK_WARNINGS")]
        private static void WarnNoSolverMulti(Type get, Type set)
        {
            Debug.LogWarning($"A generic fallback for type pair {get.Name}->{set.Name} was used." +
                             $" Implement {typeof(MultiSolver<,>).Name}<{get.Name}, {set.Name}>.");
        }
    }
}