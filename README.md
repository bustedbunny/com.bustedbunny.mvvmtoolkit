## About:

Model-View-ViewModel Toolkit for using with Unity Entities.

Main goal of this project -
bring CommunityToolkit.MVVMToolkit support into Unity ECS.

Read more about it:

https://learn.microsoft.com/en-us/windows/communitytoolkit/mvvm/introduction

## How to:

### General requirements:

* Your scene should have 1 UIDocument game object, so 
UI Initialization system will automatically use it
* UniTask if you need async request message support:
`UniTaskRequestMessage<T>`, `UniTaskCollectionRequestMessage<T>`.
You also need to define `UNITASK` in Project Setting's scripting defines.

### Make a basic view:

First we define your view and viewmodel types.

By default `ViewModel` is a baseline class to inherit from,
which inherits from `SystemBase`.

```csharp
public class MainMenuView : BaseView<MainMenuViewModel>
{
}
public partial class MainMenuViewModel : ViewModel
{
}
```

Now create a GameObject in scene with `MainMenuView` component
and attach your uxml file to it.

*You should also attach string localization table
if you want localization binding support.*

![image](https://user-images.githubusercontent.com/30902981/213801532-92c3a31a-5003-43b8-b928-b76720513067.png)

And now once you run you game,
your view is initialized and ready to be displayed.

### Enabling/Disabling View

This project relies on CommunityToolkit.MVVMToolkit Message system. 

https://learn.microsoft.com/en-us/windows/communitytoolkit/mvvm/messenger

In order to enable any View you need to implement a message:

```csharp
public class OpenMainMenuMessage { }
```

Then you will need to subscribe your ViewModel to that message:

```csharp
public partial class MainMenuViewModel : ViewModel, IRecipient<OpenMainMenuMessage>
{
    public override void OnInit()
    {
        Messenger.Register<OpenMainMenuMessage>(this);
    }
    public void Receive(OpenMainMenuMessage message)
    {
        Enable();
    }
}
```

Now you need to use Messenger and send this message:

```csharp
[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
private static async void Init()
{
    await UniTask.Yield();
    var world = World.DefaultGameObjectInjectionWorld;
    var messenger = world.GetExistingSystemManaged<UIInitializationSystem>().Messenger;
    messenger.Send<OpenMainMenuMessage>();
}
```

P.S. `UniTask.Yield()` is called in order to ensure 
it's called after UI is initialized. This will likely 
be fixed in the future.

And now your View should be displayed.

In order to disable view you can use either built in `CloseViewsMessage`
or implement your own message callbacks that will call `Disable()`
method of your `ViewModel`.

There is also a boolean property `EnabledState` so you can bind it to UI.