using System;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.UIElements;

// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable MemberCanBePrivate.Global
namespace MVVMToolkit
{
    public abstract class BaseView : MonoBehaviour
    {
        public LocalizedStringTable localizationTable;
        public VisualTreeAsset asset;
        public int sortLayer;
        public abstract Type ViewModelType { get; }
    }

    public abstract class BaseView<T> : BaseView where T : BaseViewModel
    {
        public override Type ViewModelType => typeof(T);
    }
}