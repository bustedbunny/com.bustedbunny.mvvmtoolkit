using System;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.Localization.Tables;

namespace MVVMToolkit.Binding.Localization
{
    /// <summary>
    /// Table postprocessor is obsolete and is only cleaning up for itself.
    /// Will be removed eventually
    /// </summary>
    [Serializable]
    internal class TablePostprocessor : ITablePostprocessor
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void Register()
        {
            if (LocalizationSettings.StringDatabase.TablePostprocessor is TablePostprocessor)
            {
                LocalizationSettings.StringDatabase.TablePostprocessor = null;
            }
        }

        public void PostprocessTable(LocalizationTable table) { }
    }
}