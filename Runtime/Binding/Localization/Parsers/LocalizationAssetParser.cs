using System.ComponentModel;
using UnityEngine.Localization;
using UnityEngine.UIElements;

namespace MVVMToolkit.Binding.Localization
{
    public class LocalizationAssetParser : BindingParser<LocalizedAssetBinding>
    {
        private readonly LocalizedAssetTable[] _assetTables;

        public LocalizationAssetParser(INotifyPropertyChanged binding, LocalizedAssetTable[] assetTables) : base(binding)
        {
            _assetTables = assetTables;
        }

        public override void Process(VisualElement element, string key)
        {
            throw new System.NotImplementedException();
        }

        public override char Symbol() => '#';
    }

    public class LocalizedAssetBinding : IElementBinding
    {
        public void Unbind() { }
    }
}