using System;
using System.Collections.Generic;
using System.Reflection;
using MVVMToolkit.Binding.Localization;
using UnityEngine.Localization;

namespace MVVMToolkit.Binding.Generics
{
    public interface IConversionSolver
    {
        public Type GetterType { get; }
        public Type SetterType { get; }


        public LocalizedAssetBinding ResolveBinding(PropertyInfo setProp, object target, LocalizedAssetTable table,
            string key);
    }

    public static class ConversionUtility
    {
        private static readonly Dictionary<(Type, Type), IConversionSolver> SolverMap = new();

        static ConversionUtility()
        {
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (var type in assembly.GetTypes())
                {
                    if (!type.IsAbstract && typeof(IConversionSolver).IsAssignableFrom(type))
                    {
                        var solver = (IConversionSolver)Activator.CreateInstance(type);
                        SolverMap.Add((solver.GetterType, solver.SetterType), solver);
                    }
                }
            }
        }

        public static LocalizedAssetBinding Binding(PropertyInfo setProp, object target, LocalizedAssetTable table,
            string key, Type assetType)
        {
            var setterType = setProp.PropertyType;

            if (!SolverMap.TryGetValue((assetType, setterType), out var solver))
            {
                throw new BindingException(
                    $"Couldn't find solver for pair {assetType.Name}->{setterType.Name}." +
                    $" Implement ConversionSolver<{setterType.Name}, {assetType.Name}>.");
            }

            return solver.ResolveBinding(setProp, target, table, key);
        }
    }
}