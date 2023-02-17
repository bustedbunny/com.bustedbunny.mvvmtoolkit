using System;
using UnityEngine;
using UnityEngine.Localization.Settings;

namespace MVVMToolkit.Settings
{
    // ReSharper disable once InconsistentNaming
    public class MVVMTKSettings : ScriptableObject, IReset, IDisposable
    {
        private static MVVMTKSettings _instance;

        public static MVVMTKSettings Instance
        {
            get
            {
                if (ReferenceEquals(_instance, null))
                {
                    _instance = GetOrDefault();
                }

                return _instance;
            }
            set => _instance = value;
        }

        public readonly long tooltipHoverTime = 500;

        private static MVVMTKSettings GetOrDefault()
        {
            var instance = GetInstance();

            if (ReferenceEquals(_instance, null))
            {
                Debug.LogWarning("Could not find localization settings. Default will be used.");

                instance = CreateInstance<MVVMTKSettings>();
                instance.name = "Default MVVMTK Settings";
            }

            return instance;
        }

        internal const string ConfigName = "com.bustedbunny.mvvmtoolkit";

        private static MVVMTKSettings GetInstance()
        {
            if (!ReferenceEquals(_instance, null))
            {
                return _instance;
            }

            MVVMTKSettings instance;
            #if UNITY_EDITOR
            UnityEditor.EditorBuildSettings.TryGetConfigObject(ConfigName, out instance);
            #else
            instance = FindObjectOfType<MVVMTKSettings>();
            #endif
            return instance;
        }

        public void ResetState() { }

        public void Dispose() { }
    }
}