using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.Localization.SmartFormat.Core.Extensions;
using UnityEngine.Localization.SmartFormat.PersistentVariables;

namespace MVVMToolkit.Binding.Localization.Source
{
    public class BindingSource : ISource, IDisposable
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
        private static void Init()
        {
            var extensions = LocalizationSettings.StringDatabase.SmartFormatter.SourceExtensions;
            if (extensions.OfType<BindingSource>().FirstOrDefault() == default)
            {
                extensions.Insert(0, new BindingSource());
            }

            LocalizationSettings.StringDatabase.SmartFormatter.Parser.AddOperators("#>");
        }

        #if UNITY_EDITOR
        [UnityEditor.InitializeOnLoadMethod]
        private static void EditorInit()
        {
            UnityEditor.EditorApplication.playModeStateChanged += OnQuit;
        }

        private static void OnQuit(UnityEditor.PlayModeStateChange obj)
        {
            if (obj == UnityEditor.PlayModeStateChange.ExitingPlayMode)
                LocalizationSettings.StringDatabase.SmartFormatter.SourceExtensions.RemoveAll(source =>
                    source is BindingSource);
        }
        #endif


        public void Dispose()
        {
            LocalizationSettings.StringDatabase?.SmartFormatter?.SourceExtensions.Remove(this);
        }

        public bool TryEvaluateSelector(ISelectorInfo selectorInfo)
        {
            var selectorOperator = selectorInfo.SelectorOperator;
            if (string.IsNullOrEmpty(selectorOperator) || selectorOperator.Length != 1) return false;
            var symbol = selectorOperator[0];
            if (symbol is not '>' && symbol is not '#') return false;

            var bindingGroup = selectorInfo.CurrentValue as BindingGroup;
            if (bindingGroup is null && selectorInfo.FormatDetails?.OriginalArgs is not null)
            {
                foreach (var arg in selectorInfo.FormatDetails.OriginalArgs)
                {
                    if (arg is not BindingGroup group) continue;
                    bindingGroup = group;
                    break;
                }

                // Fallback if argument is not defined (smth is wrong)
                selectorInfo.Result = string.Empty;
                return true;
            }


            // # is used for local variables
            if (symbol is '>')
            {
                if (!bindingGroup.BindVariable(selectorInfo.SelectorText, out var variable)) return false;
                CacheAndSetResult(selectorInfo, variable);
                return true;
            }

            if (symbol is '#')
            {
                if (!bindingGroup.BindGroup(selectorInfo.SelectorText, out var group)) return false;

                CacheAndSetResult(selectorInfo, group);
                return true;
            }

            return false;
        }

        private static void CacheAndSetResult(ISelectorInfo info, IVariableValueChanged variable)
        {
            // Add the variable to the cache
            var cache = info.FormatDetails.FormatCache;
            if (cache != null)
            {
                if (!cache.VariableTriggers.Contains(variable))
                    cache.VariableTriggers.Add(variable);
            }

            info.Result = variable.GetSourceValue(info);
        }
    }
}