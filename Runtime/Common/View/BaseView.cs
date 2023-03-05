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
        [SerializeField] private LocalizedAssetTable[] _localizationAssetTables;
        public LocalizedAssetTable[] LocalizedAssetTables => _localizationAssetTables;

        [FormerlySerializedAs("localizationTables")] [SerializeField]
        private LocalizedStringTable[] _localizationStringTables;

        public LocalizedStringTable[] LocalizedStringTables => _localizationStringTables;
        [SerializeField] private VisualTreeAsset asset;
        protected VisualTreeAsset Asset => asset;
        [SerializeField] private int sortLayer;
        public int SortLayer => sortLayer;

        [SerializeField] private ViewModel bindingContext;
        protected ViewModel BindingContext => bindingContext;
        protected BindingParser BindingParser { get; set; }
        public VisualElement RootVisualElement { get; private set; }

        protected virtual void Awake() => RootVisualElement = Instantiate();

        public const string RootUssClassName = "mvvmtk-root-view-base";

        protected VisualElement InstantiateAsset()
        {
            var root = asset.Instantiate();
            root.pickingMode = PickingMode.Ignore;
            root.name = gameObject.name;
            return root;
        }

        protected virtual VisualElement Instantiate()
        {
            var root = InstantiateAsset();
            root.AddToClassList(RootUssClassName);
            return root;
        }

        protected virtual BindingParser ResolveBinding() =>
            new(BindingContext, RootVisualElement, LocalizedStringTables, LocalizedAssetTables);

        public void Initialize()
        {
            if (BindingContext is not null)
            {
                BindingParser = ResolveBinding();
            }

            if (enabled) OnEnable();
            else OnDisable();
        }

        public virtual VisualElement ResolveParent() => null;


        public const string EnabledClassName = "mvvmtk-enabled";
        public const string DisabledClassName = "mvvmtk-disabled";

        protected virtual void OnEnable()
        {
            RootVisualElement.AddToClassList(EnabledClassName);
            RootVisualElement.RemoveFromClassList(DisabledClassName);
        }

        protected virtual void OnDisable()
        {
            RootVisualElement.AddToClassList(DisabledClassName);
            RootVisualElement.RemoveFromClassList(EnabledClassName);
        }

        protected virtual void OnDestroy()
        {
            RootVisualElement?.parent?.Remove(RootVisualElement);
            BindingParser?.Dispose();
            Messenger?.UnregisterAll(this);
        }

        public void Receive(CloseViewsMessage message) => enabled = false;
    }
}