using System;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.Localization.SmartFormat.Core.Extensions;
using UnityEngine.Localization.Tables;

namespace MVVMToolkit.Binding.Localization
{
    [Serializable]
    public class Selector : ISource
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void Init()
        {
            var selector = LocalizationSettings.StringDatabase.SmartFormatter.GetSourceExtension<Selector>();
            if (selector == null)
                LocalizationSettings.StringDatabase.SmartFormatter.SourceExtensions.Add(new Selector());
        }

        public bool TryEvaluateSelector(ISelectorInfo selectorInfo)
        {
            if (selectorInfo.SelectorText == "#")
            {
                selectorInfo.Result = string.Empty;
                return true;
            }

            return false;
        }
    }

    public delegate void TableLoadedEvent(string tableCollectionName);

    [Serializable]
    public class TablePostprocessor : ITablePostprocessor
    {
        private static ITablePostprocessor _previousValue;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void Register()
        {
            _previousValue = LocalizationSettings.StringDatabase.TablePostprocessor;
            LocalizationSettings.StringDatabase.TablePostprocessor = new TablePostprocessor();

            LocalizationSettings.StringDatabase.SmartFormatter.Parser.AddOperators("#");
        }

        public static event TableLoadedEvent OnTableLoaded;

        public void PostprocessTable(LocalizationTable table)
        {
            OnTableLoaded?.Invoke(table.TableCollectionName);

            if (_previousValue is not null && _previousValue is not TablePostprocessor)
            {
                _previousValue?.PostprocessTable(table);
            }
        }
    }
}