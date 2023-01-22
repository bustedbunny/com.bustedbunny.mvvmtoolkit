using System;
using CommunityToolkit.Mvvm.Messaging;
using Unity.Entities;
using UnityEngine;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

namespace MVVMToolkit
{
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    public partial class UIInitializationSystem : SystemBase
    {
        private readonly UISingleton _uiSingleton = new();
        public StrongReferenceMessenger Messenger { get; } = new();

        protected override void OnCreate()
        {
            _uiSingleton.Document = Object.FindObjectOfType<UIDocument>();
            _uiSingleton.Messenger = Messenger;
            var e = EntityManager.CreateSingleton<UISingleton>();
            EntityManager.AddComponentObject(e, _uiSingleton);
        }

        private static void Warning()
        {
            Debug.LogWarning("Couldn't load UIDocument. Ensure there is a valid UIDocument in a scene.");
        }


        protected override void OnStartRunning()
        {
            Enabled = false;

            if (_uiSingleton.Document is null)
            {
                var doc = Object.FindObjectOfType<UIDocument>();
                if (doc is null) Warning();

                _uiSingleton.Document = doc;
            }

            var array = Object.FindObjectsOfType<BaseView>();

            // ReSharper disable once PossibleInvalidCastExceptionInForeachLoop
            foreach (var view in array)
            {
                try
                {
                    var type = view.ViewModelType;
                    var viewModel = (BaseViewModel)World.GetExistingSystemManaged(type);

                    var original = view.asset.Instantiate();
                    original.style.flexGrow = 1f;
                    original.pickingMode = PickingMode.Ignore;
                    viewModel.Bind(new OrderedContainer(original, view.sortLayer), _uiSingleton, view, Messenger);
                    viewModel.OnInit();
                    viewModel.PostInit();

                    // _uiSingleton.callbacks.AddRange(StateProcessor.Parse(viewModel));
                    _uiSingleton.viewModels.Add(viewModel);
                }
                catch (Exception exception)
                {
                    Debug.LogException(exception);
                }
            }
        }

        protected override void OnUpdate() { }
    }

    public class OrderedContainer : VisualElement
    {
        public int Order { get; }

        public OrderedContainer(VisualElement child, int order)
        {
            pickingMode = PickingMode.Ignore;
            Add(child);
            Order = order;
        }
    }
}