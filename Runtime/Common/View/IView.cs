using MVVMToolkit.Binding;
using UnityEngine.Localization;
using UnityEngine.UIElements;

namespace MVVMToolkit
{
    public interface IView
    {
        LocalizedStringTable LocalizedTable { get; }
        VisualTreeAsset Asset { get; }
        int SortLayer { get; }
        ViewModel BindingContext { get; }
        BindingParser BindingParser { get; }
        VisualElement RootVisualElement { get; }

        public VisualElement Instantiate();
        public VisualElement ResolveParent();
        public BindingParser ResolveBinding();
        
    }
}