## About:

Model-View-ViewModel Toolkit for using with Unity Entities.

Main goal of this project -
bring CommunityToolkit.MVVMToolkit support into Unity ECS.

[Read more about CommunityToolkit.MVVM](https://learn.microsoft.com/en-us/windows/communitytoolkit/mvvm/introduction)

## Table of contents

- [General requirements](#general-requirements)
- [Make a basic view](#make-a-basic-view)
- [Enabling/Disabling View](#enablingdisabling-view)
- [Localization text binding](#localization-text-binding)
- [Input binding](#input-binding)
- [Smart-strings localization support](#smart-strings-localization-support)

## How to:

### General requirements

* Your scene should have 1 UIDocument game object, so
  UI Initialization system will automatically use it
* UniTask if you need async request message support:
  `UniTaskRequestMessage<T>`, `UniTaskCollectionRequestMessage<T>`.
  You also need to define `UNITASK` in Project Setting's scripting defines.

### Make a basic view

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

## Binding

### Localization text binding

In order to bind `TextElement`'s `text` value to localized string you need:

1. Create localization string table using Unity's localization package.
   ![image](https://user-images.githubusercontent.com/30902981/213808156-21abb906-4686-473a-9587-fa5d0e133d65.png)
2. Assign that table to your View's localization table field.
3. Assign `text` attribute of `TextElement` in `.uxml` to
   required key with `#` selector.

```uxml

<ui:Button text="#StartNewGame" name="StartNewGame"/>
<ui:Button text="#HowToPlay" name="HowToPlay"/>
<ui:Button text="#Settings" name="Settings"/>
```

Binding automatically updates `text` value on every table change.
For example if you switch language.

### Input binding

To bind a button to specific command you will need to
implement a parameterless void method with `[RelayCommand]` attribute:

```csharp
[RelayCommand]
private void StartNewGame()
{
}
```

This will generate an `IRelayCommand StartNewGameCommand` property
with this `StartNewGame` method as callback.

In associated `.uxml` add `@StartNewGameCommand` to text attribute of your
button (It doesn't have to be `Button` class, every `TextElement` can be
bound to commands).

```uxml

<ui:Button text="#StartNewGame;@StartNewGameCommand" name="StartNewGame"/>
```

*Different bindings in `text` attribute are separated by `;` character.*

You can also bind button's enabled state to boolean.

```csharp
[ObservableProperty, NotifyCanExecuteChangedFor(nameof(StartNewGameCommand))]
private bool _canStartNewGame;

[RelayCommand(CanExecute = nameof(CanStartNewGame))]
private void StartNewGame(){}
```

Now button's enabled state will be bound to
generated `CanStartNewGame` property.

### Smart-strings localization support

Define property name in default Smart-string braces.

![image](https://user-images.githubusercontent.com/30902981/213810482-875ad5d0-e5a7-4dd4-adfa-5acb36b54564.png)

Define public properties in your `ViewModel`.

```csharp
public int WorkerCost => MinionCount.WorkerCost;
public int MilitaryCost => MinionCount.MilitaryCost;
public int ScientistCost => MinionCount.ScientistCost;
```

Define localized string's keys in `text` attribute of your `TextElement`.

```uxml

<ui:Label text="#HireMenuWorkersLabel"/>
<ui:Label text="#HireMenuMilitaryLabel"/>
<ui:Label text="#HireMenuScientistsLabel"/>
```

Proper `LocalizedString`'s variables will automatically created and updated as property value changes.

*For now nested variable is unsupported.
For example: `{Scientist:{Name}}` will not work and will likely throw error during binding.`*