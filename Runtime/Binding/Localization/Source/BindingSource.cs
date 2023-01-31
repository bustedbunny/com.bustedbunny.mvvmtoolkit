using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.Localization.SmartFormat.Core.Extensions;

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
            var bindingGroup = selectorInfo.CurrentValue as BindingGroup ??
                               selectorInfo.FormatDetails.OriginalArgs.OfType<BindingGroup>().FirstOrDefault();
            if (bindingGroup is null)
                return false;

            var symbol = selectorInfo.SelectorOperator;

            // # is used for local variables
            if (symbol[0] is '>')
            {
                if (!bindingGroup.BindVariable(selectorInfo.SelectorText, out var variable)) return false;
                // Add the variable to the cache
                var cache = selectorInfo.FormatDetails.FormatCache;
                if (cache != null)
                {
                    if (!cache.VariableTriggers.Contains(variable))
                        cache.VariableTriggers.Add(variable);
                }

                selectorInfo.Result = variable.GetSourceValue(selectorInfo);
                return true;
            }

            if (symbol[0] is '#')
            {
                if (!bindingGroup.BindGroup(selectorInfo.SelectorText, out var group)) return false;
                // Add the variable to the cache
                var cache = selectorInfo.FormatDetails.FormatCache;
                if (cache != null)
                {
                    if (!cache.VariableTriggers.Contains(group))
                        cache.VariableTriggers.Add(group);
                }

                selectorInfo.Result = group.GetSourceValue(selectorInfo);
                return true;
            }

            return false;
        }
    }
}