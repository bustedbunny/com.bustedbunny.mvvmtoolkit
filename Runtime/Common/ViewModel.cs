using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using MVVMToolkit.Binding;
using MVVMToolkit.Messaging;
using Unity.Entities;
using UnityEngine.UIElements;

namespace MVVMToolkit
{
    public abstract partial class ViewModel : BaseViewModel<BaseView> { }

    public abstract partial class BaseViewModel<T> : BaseViewModel where T : BaseView
    {
        public T SourceView { get; private set; }

        internal override void Bind(VisualElement view, UISingleton uiSingleton, BaseView sourceView,
            StrongReferenceMessenger messenger)
        {
            base.Bind(view, uiSingleton, sourceView, messenger);
            SourceView = (T)sourceView;
        }
    }

    [ObservableObject]
    public abstract partial class BaseViewModel : SystemBase, IRecipient<CloseScreensMessage>
    {
        protected UISingleton UISingleton { get; private set; }
        public VisualElement RootVisualElement { get; private set; }
        protected StrongReferenceMessenger Messenger { get; private set; }

        protected override void OnDestroy()
        {
            Messenger?.UnregisterAll(this);
            _bindingParser?.Dispose();
        }

        protected override void OnUpdate()
        {
            Enabled = false;
        }

        public virtual void OnInit() { }

        private BaseView _source;

        internal virtual void Bind(VisualElement view, UISingleton uiSingleton, BaseView sourceView,
            StrongReferenceMessenger messenger)
        {
            Messenger = messenger;
            Messenger.Register(this);

            _source = sourceView;

            UISingleton = uiSingleton;
            RootVisualElement = view;
            // This is done in order to avoid clipping
            RootVisualElement.pickingMode = PickingMode.Ignore;
        }


        private BindingParser _bindingParser;

        public void PostInit()
        {
            _bindingParser = new BindingParser(this, RootVisualElement, _source.localizationTable);
        }

        /// <summary>
        /// Helper method to reduce garbage. Equals `EnabledState = true`.
        /// </summary>
        public void Enable()
        {
            EnabledState = true;
        }

        /// <summary>
        /// Helper method to reduce garbage. Equals `EnabledState = false`.
        /// </summary>
        public void Disable()
        {
            EnabledState = false;
        }

        [ObservableProperty] private bool _enabledState;

        partial void OnEnabledStateChanged(bool value)
        {
            if (value)
            {
                UISingleton.Add(this);
                OnEnable();
            }
            else
            {
                UISingleton.Remove(this);
                OnDisable();
            }
        }

        protected virtual void OnEnable() { }

        protected virtual void OnDisable() { }


        public void Receive(CloseScreensMessage message)
        {
            Disable();
        }
    }
}