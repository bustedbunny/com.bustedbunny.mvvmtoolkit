using CommunityToolkit.Mvvm.Messaging;
using Cysharp.Threading.Tasks;
using MVVMToolkit;
using UnityEngine;

// This is just an example of how you can initialize your UI
// This can be done in infinite other ways and depends only on your application
[RequireComponent(typeof(UIRoot))]
public class SampleUIInitializer : MonoBehaviour
{
    // Internally UI is initialized in Awake
    // Actual initialization should be done at least after Start 
    void Start() => InitializeAsync().Forget();

    private async UniTask InitializeAsync()
    {
        var root = GetComponent<UIRoot>();

        // We call UIRoot.Initialize method and provide StrongReferenceMessenger and ServiceProvider instances.
        // If you have external services on which your Views or ViewModels rely you must register them
        // before calling Initialize.
        var messenger = new StrongReferenceMessenger();

        // Before we can make any calls to UI, we need to await it's initialization
        await root.Initialize(messenger, new());

        messenger.Send<OpenTestViewMessage>();
    }
}