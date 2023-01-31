using System;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.Localization.Tables;

namespace MVVMToolkit.Binding.Localization
{
    public delegate void TableLoadedEvent();

    /// <summary>
    /// Table postprocessor provides static event for localization binding to analyze tables before values are evaluated
    /// and errors about unresolved variables is thrown
    /// </summary>
    [Serializable]
    internal class TablePostprocessor : ITablePostprocessor
    {
        private static ITablePostprocessor _previousValue;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void Register()
        {
            _previousValue = LocalizationSettings.StringDatabase.TablePostprocessor;
            LocalizationSettings.StringDatabase.TablePostprocessor = new TablePostprocessor();

            LocalizationSettings.StringDatabase.SmartFormatter.Parser.AddOperators("#");
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ReloadStatic()
        {
            if (OnTableLoaded is not null)
            {
                var kek = OnTableLoaded.GetInvocationList();
                foreach (var del in kek)
                {
                    OnTableLoaded -= (TableLoadedEvent)del;
                }
            }
        }

        public static event TableLoadedEvent OnTableLoaded;

        public void PostprocessTable(LocalizationTable table)
        {
            OnTableLoaded?.Invoke();

            if (_previousValue is not null && _previousValue is not TablePostprocessor)
            {
                _previousValue?.PostprocessTable(table);
            }
        }
    }
}