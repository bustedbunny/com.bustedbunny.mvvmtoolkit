// using System;
// using System.Collections.Generic;
// using CommunityToolkit.Mvvm.Messaging;
// using Unity.Entities;
// using UnityEngine;
// using UnityEngine.UIElements;
// using Object = UnityEngine.Object;
//
// namespace MVVMToolkit
// {
//     [UpdateInGroup(typeof(InitializationSystemGroup))]
//     public partial class UIInitializationSystem : SystemBase
//     {
//         private readonly UISingleton _uiSingleton = new();
//         public StrongReferenceMessenger Messenger { get; } = new();
//
//         public ServiceProvider ServiceProvider { get; private set; }
//
//         protected override void OnCreate()
//         {
//             ServiceProvider = new ServiceProvider(World, Messenger);
//             _uiSingleton.root = new VisualElement();
//             _uiSingleton.root.style.flexGrow = new StyleFloat(1f);
//             _uiSingleton.Document = Object.FindObjectOfType<UIDocument>();
//             _uiSingleton.Messenger = Messenger;
//             var e = EntityManager.CreateSingleton<UISingleton>();
//             EntityManager.AddComponentObject(e, _uiSingleton);
//         }
//
//         private static void Warning()
//         {
//             Debug.LogWarning("Couldn't load UIDocument. Ensure there is a valid UIDocument in a scene.");
//         }
//
//         private void InitViewModels()
//         {
//             foreach (var viewModel in Object.FindObjectsOfType<ViewModel>())
//             {
//                 _uiSingleton.viewModels.Add(viewModel.GetType(), viewModel);
//                 viewModel.Init(_uiSingleton, Messenger, ServiceProvider);
//             }
//         }
//
//
//         protected override void OnStartRunning()
//         {
//             Enabled = false;
//
//             ServiceProvider.OnSceneLoaded();
//             InitViewModels();
//
//             if (_uiSingleton.Document is null)
//             {
//                 var doc = Object.FindObjectOfType<UIDocument>();
//                 if (doc is null) Warning();
//
//                 _uiSingleton.Document = doc;
//             }
//
//             var array = Object.FindObjectsOfType<BaseView>();
//             var list = new List<VisualElement>(array.Length);
//             var sortDict = new Dictionary<VisualElement, int>(array.Length);
//
//             foreach (var view in array)
//             {
//                 var cont = view.Instantiate();
//                 list.Add(cont);
//                 sortDict.Add(cont, view.sortLayer);
//             }
//
//             var root = _uiSingleton.root;
//
//             for (var i = 0; i < array.Length; i++)
//             {
//                 var element = list[i];
//                 var view = array[i];
//
//                 var viewModel = _uiSingleton.viewModels[view.ViewModelType];
//                 var runtime = new RuntimeView(view, element, viewModel);
//                 viewModel.Bind(runtime);
//             }
//
//             for (int i = 0; i < array.Length; i++)
//             {
//                 var element = list[i];
//                 var view = array[i];
//
//                 var parent = view.ResolveParent(list) ?? root;
//                 parent.Add(element);
//                 parent.Sort((x, y) =>
//                 {
//                     sortDict.TryGetValue(x, out var xSort);
//                     sortDict.TryGetValue(y, out var ySort);
//                     return Comparer<int>.Default.Compare(xSort, ySort);
//                 });
//             }
//
//             foreach (var (_, viewModel) in _uiSingleton.viewModels)
//             {
//                 try
//                 {
//                     viewModel.OnInit();
//                 }
//                 catch (Exception e)
//                 {
//                     Debug.LogException(e);
//                 }
//             }
//         }
//
//         protected override void OnUpdate() { }
//     }
// }