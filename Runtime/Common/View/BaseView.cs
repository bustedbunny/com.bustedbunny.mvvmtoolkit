using CommunityToolkit.Mvvm.Messaging;
using MVVMToolkit.Binding;
using MVVMToolkit.Messaging;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Serialization;
using UnityEngine.UIElements;

// ReSharper disable MemberCanBePrivate.Global

namespace MVVMToolkit
{
    public class BaseView : UIObject, IRecipient<CloseViewsMessage>
    {
        [FormerlySerializedAs("localizationTable")] [SerializeField]
        private LocalizedStringTable[] localizationTables;

        protected LocalizedStringTable[] LocalizedTables => localizationTables;
        [SerializeField] private VisualTreeAsset asset;
        protected VisualTreeAsset Asset => asset;
        [SerializeField] private int sortLayer;
        public int SortLayer => sortLayer;

        [SerializeField] private ViewModel bindingContext;
        protected ViewModel BindingContext => bindingContext;
        protected BindingParser BindingParser { get; set; }
        public VisualElement RootVisualElement { get; private set; }

        protected virtual void Awake() => RootVisualElement = Instantiate();

        protected virtual VisualElement Instantiate()
        {
            var root = asset.Instantiate();
            // root.style.flexGrow = new StyleFloat(1f);
            root.style.height = new StyleLength(Length.Percent(100f));
            root.style.width = new StyleLength(Length.Percent(100f));
            root.style.position = new StyleEnum<Position>(Position.Absolute);
            return root;
        }

        protected virtual BindingParser ResolveBinding() =>
            new BindingParser(BindingContext, RootVisualElement, LocalizedTables);

        public void Initialize()
        {
            if (BindingContext is not null)
            {
                BindingParser = ResolveBinding();
            }

            enabled = false;
        }

        public virtual VisualElement ResolveParent() => null;


        protected virtual void OnEnable() =>
            RootVisualElement.style.display = new StyleEnum<DisplayStyle>(DisplayStyle.Flex);

        protected virtual void OnDisable() =>
            RootVisualElement.style.display = new StyleEnum<DisplayStyle>(DisplayStyle.None);

        protected virtual void OnDestroy()
        {
            RootVisualElement?.parent?.Remove(RootVisualElement);
            BindingParser?.Dispose();
            Messenger?.UnregisterAll(this);
        }

        public void Receive(CloseViewsMessage message) => enabled = false;
    }
}