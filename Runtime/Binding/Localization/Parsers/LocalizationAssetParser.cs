using System.ComponentModel;
using UnityEngine.Localization;
using UnityEngine.UIElements;

namespace MVVMToolkit.Binding.Localization
{
    public class LocalizationAssetParser : BindingParser<LocalizedAssetBinding>
    {
        private readonly LocalizedAssetTable[] _assetTables;

        public LocalizationAssetParser(INotifyPropertyChanged binding, LocalizedAssetTable[] assetTables) :
            base(binding)
        {
            _assetTables = assetTables;
        }

        public override char Symbol() => '#';
        public override void Process(VisualElement element, string key) { }
    }

    public class LocalizedAssetBinding : IElementBinding
    {
        public LocalizedAssetBinding(INotifyPropertyChanged binding, LocalizedAssetTable table, string key)
        {
            var kek = table.GetTable();
        }

        public void Unbind() { }
    }
}