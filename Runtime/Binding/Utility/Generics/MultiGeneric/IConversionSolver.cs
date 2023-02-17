using System;
using System.Collections.Generic;
using System.Reflection;
using MVVMToolkit.Binding.Localization;
using UnityEngine.Localization;

namespace MVVMToolkit.Binding.Generics
{
    public interface IConversionSolver
    {
        public Type SetType { get; }


        public LocalizedAssetBinding ResolveBinding(PropertyInfo setProp, object target, LocalizedAssetTable table,
            string key);
    }

    public static class ConversionUtility
    {
        private static readonly Dictionary<Type, IConversionSolver> SolverMap = new();

        static ConversionUtility()
        {
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (var type in assembly.GetTypes())
                {
                    if (!type.IsAbstract && typeof(IConversionSolver).IsAssignableFrom(type))
                    {
                        var solver = (IConversionSolver)Activator.CreateInstance(type);
                        SolverMap.Add(solver.SetType, solver);
                    }
                }
            }
        }

        public static LocalizedAssetBinding Binding(PropertyInfo setProp, object target, LocalizedAssetTable table,
            string key)
        {
            var type = setProp.PropertyType;

            if (!SolverMap.TryGetValue(type, out var solver))
            {
                throw new BindingException(
                    $"Couldn't find solver for type {type.Name}. Implement one to use Asset Binding.");
            }

            return solver.ResolveBinding(setProp, target, table, key);
        }
    }
}