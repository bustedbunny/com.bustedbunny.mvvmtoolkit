## About:

Model-View-ViewModel Toolkit for using with Unity's UI Toolkit.

Main goal of this project - bring MVVM into UI Toolkit.

Reminder: this is WIP and until first stable release
many breaking changes might be pushed without a warning.

[Read more about CommunityToolkit.MVVM](https://learn.microsoft.com/en-us/dotnet/communitytoolkit/mvvm/)

## Table of contents

- [General requirements](#general-requirements)
- [Make a basic view](#make-a-basic-view)
- [Enabling/Disabling View](#enablingdisabling-view)
- [Localization text binding](#localization-text-binding)
- [Smart-string binding](#smart-string-binding)
- [Input binding](#input-binding)
- [Value Changed binding](#value-changed-binding)
- [Reflection binding](#reflection-binding)
- [String format binding](#string-format-binding)
- [Smart-String nested variables](#smart-string-nested-variables)
- [Nested Localized String binding](#nested-localized-string-binding)
- [Burst Messenger support](#burst-messenger-support)
- [Localization Asset Binding](#localization-asset-binding)
- [Binding Types Limitations](#binding-types-limitations)

## Roadmap

* ~~Tooltip text binding.~~ ~~Tooltips don't work in runtime :(~~
* ~~Tooltips extension.~~ Done.
* ~~Nested variable support for smart-string~~ Done.
* ~~Burst-compatible wrapper for Messenger.~~ Done.
* ~~Localization Asset Table support.~~ Done.
* ~~Localization Multi-Table support.~~ Done.
* ~~Binding type conversion support.~~ Done.

## How to:

### General requirements

* [UniTask](https://github.com/Cysharp/UniTask). It is used widely to provide spike less Localization's string
  generation.
* Net Standard 2.1
* Unity 2022.2+. While all previous version are also partially supported
  (as long as they support NS 2.1 and UniTask),
  2022.2 also supports Roslyn 4.0.1 API which gives an
  opportunity to use all power of CommunityToolkit.mvvm source generators.
* '#', '>' and '@' symbols are reserved in Localization package operators

### Make a basic view

First we define viewmodel type.

`ViewModel` is a baseline class to inherit from,
which inherits from `MonoBehavior`.

```csharp
public class TestView : BaseView
{
}
public partial class TestViewModel : ViewModel
{
}
```

Now we create a GameObject in scene with `TestView` and `TestViewModel`
components and attach our uxml asset to proper field.

In a BindingContext we assign our ViewModel.

*We should also attach our string localization table to
support localization binding.*

![image](https://user-images.githubusercontent.com/30902981/215566294-47771601-5efe-45c4-bff4-2acb03478b04.png)

Now we need to define our UI hierarchy.

We create game object with `UIRoot` component attached.

Here we assign our `UIDocument` we want to use.

*Although it is not required and we can attach `UIDocument` later.
We can even remove it.
Our UI hierarchy will be automatically attached to it's root.*

![image](https://user-images.githubusercontent.com/30902981/215566632-06057a9e-b78a-4176-955d-6b03edc4f330.png)

Now we need to assign your `View` game object as a child of `UIRoot`.

![image](https://user-images.githubusercontent.com/30902981/215567898-33314ddf-4d61-41cf-ac78-a771b6979554.png)

So far our scene looks like this and now we need to `Initialize` our `UIRoot`.

Let's create a simple script to do so:

```csharp
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
        var serviceProvider = new ServiceProvider();

        // Before we can make any calls to UI, we need to await it's initialization
        await root.Initialize(messenger, serviceProvider);

        messenger.Send<OpenTestViewMessage>();
    }
}
```

After we attached this script to `UIRoot` we can start PlayMode, but View will not appear.

### Enabling/Disabling View

This framework heavily relies on CommunityToolkit.MVVMToolkit Message system.

[You can read more here.](https://learn.microsoft.com/en-us/dotnet/communitytoolkit/mvvm/messenger)

In order to enable any `View` we will need to implement a message:

```csharp
public class OpenTestViewMessage { }
```

Then we will need to subscribe our `View` to that message. There are several ways to do so:

1. First we can simply inherit from `IRecipient<T>`. Our `View` will be automatically subscribed.

```csharp
public class TestView : BaseView, IRecipient<OpenTestViewMessage>
{
    // On message receive we will enable our View 
    public void Receive(OpenTestViewMessage message)
    {
        enabled = true;
    }
}
```

2. Alternatively we can subscribe manually.

```csharp
public class TestView : BaseView
{
    // You can override OnInit to make manual changes to hierarchy
    protected override void OnInit()
    {
        Messenger.Register<OpenTestViewMessage>(this, (recipient, message) => enabled = true);
    }
}
```

Now we need to use Messenger and send this message:

```csharp
public class UIInitializer : MonoBehaviour
{
    void Start()
    {
        ...
        
        await root.Initialize(messenger, serviceProvider);
        messenger.Send<OpenTestViewMessage>();
    }
}
```

*Messenger should only be used after `UIRoot` is Initialized.*

And now our `View` should be displayed on PlayMode.

In order to disable view we can use either built in `CloseViewsMessage`
or implement your own message callbacks that will call `enabled = false`
method of our `View`.

## Binding

### Localization text binding

In order to bind `TextElement`'s `text` value to localized string we need:

1. Create localization string table using Unity's localization package.
2. Assign that table to your View's localization table field.
3. Assign `text` attribute of `TextElement` in `.uxml` to
   required key with `#` operator.

![image](https://user-images.githubusercontent.com/30902981/215570528-f8c66110-0911-47b3-a77c-c572f90898cf.png)

```xml

<ui:Label text="#Text"/>
```

Binding automatically updates `text` value on every table change.
For example when we switch language or when we modify table in editor.

_You can also bind tooltips with this, using `tooltip` attribute._

### Smart-string binding

Now we want to display some variables.

We need to make our Localization entry `Smart` and define variables with `>` operator.
Variable name must match Property on `BindingContext` (`ViewModel` our `View` is attached to).

![image](https://user-images.githubusercontent.com/30902981/215571532-e4fd0421-7a35-4620-b6a9-eab813faaa31.png)

_You can also bind tooltips with this, using `tooltip` attribute._

```csharp
public partial class TestViewModel : ViewModel
{
    // To bind a simple property, just create a backing field
    // and attach [ObservableProperty] attribute. TestInt property will be generated.
    [ObservableProperty] private int _testInt = 12;
}
```

In our `.uxml` asset we define `text` attribute with entry's key with `#` operator.

```xml

<ui:Label text="#VariableTest"/>
```

Now our `View` will be automatically updated as we change `TestInt` property value.

### Input binding

To bind a button to specific method we will need to
implement a void method
with `[RelayCommand]` attribute or create `ICommand` property ourselves.

Let's create a simple counter:

```csharp
public partial class TestViewModel : ViewModel
{
    // To bind a method to click event you will need ICommand property.
    // [RelayCommand] will automatically generate it for you.
    [RelayCommand]
    private void Increment() => Counter++;
    [ObservableProperty]
    private int _counter;

}
```

In our `.uxml` asset we define `view-data-key` attribute. Each binding needs to be wrapped in braces.
`ICommand` binding requires `@` operator.

```xml

<ui:Button text="Counter" view-data-key="{@IncrementCommand}"/>
```

We can also bind button's enabled state to boolean field/property/method,
which will be updated automatically.

```csharp
[RelayCommand(CanExecute = nameof(CanIncrement))]
```

We can also send bool, int, float or string as parameter.

```xml

<ui:Button view-data-key="{@FooCommand:5}"/>
```

```csharp
[RelayCommand]
private void Foo(int arg) 
{
// arg is going to be 5
}
```

### Value Changed binding

In order to bind elements with `INotifyValueChanged<T>` we will need to implement a property with
matching type.

Value Changed binding uses `%` operator.

```xml

<ui:IntegerField label="Counter" view-data-key="{%Counter}"/>
```

Now `Counter` property value will be mirrored
if you manually type a value into field.

### Reflection binding

Sometimes we want to bind something very custom and specific.
In order to do so we can use reflection binding.

Reflection binding uses `^` operator.

```xml

<ui:Label text="This text font size is bound to Counter" view-data-key="{^style.fontSize=FontSize}"/>
```

In ViewModel we will need to define matching type.

```csharp
public partial class TestViewModel : ViewModel
{
    // In some scenarios multiple properties can be attached to one backing field.
    // In this case use [NotifyPropertyChangedFor] attribute
    [ObservableProperty, NotifyPropertyChangedFor(nameof(FontSize))]
    private int _counter;

    // This property provides a proper type for VisualElement.style.fontSize for binding
    public StyleLength FontSize => Counter;
}
```

Now as we modify our `Counter` value, this `Label`'s fontSize will
also change accordingly.

### String format binding

Sometimes we don't need localization and we just want to bind string.

String format binding uses `$` operator.

In `.uxml` we simply define our format the same way as we do in C#.
Matching properties must exist in bound `ViewModel`.

```xml

<ui:Label text="$Counter={Counter}"/>
```

### Smart-String nested variables

Nested variables are fully supported.

To define a group we need to use `#` operator and `>` operator for a variable.

![image](https://user-images.githubusercontent.com/30902981/215578142-70b12a58-7132-4e20-87e2-628c8a694f28.png)

With full support we can go fully nuts:

![image](https://user-images.githubusercontent.com/30902981/215578542-9e86061c-3947-44e8-82de-7026b0bf98a2.png)

### Nested Localized String binding

In order to nest LocalizedString inside another one use '@' operator.

![image](https://user-images.githubusercontent.com/30902981/217531299-b111ece9-4a49-4559-a0a5-8b9e11389841.png)

### Burst Messenger support

In case you want Burst code to send messages with data first declare
a type inheriting from `IUnmanagedMessage`.

```csharp
private struct TestInt : IUnmanagedMessage
{
    public int value;
}
```

Now we need to create `WrapperReference` using
our `Messenger` instance as argument.

```csharp
var messenger = new StrongReferenceMessenger();
var wrapper = new WrapperReference(messenger);
```

After that we can pass struct obtained from `wrapper.Wrapper` to our
unmanaged code and call `Send` method on it. For example:

```csharp
var value = new TestInt { value = 62 };
Wrapper.Send(value);
```

Once our unmanaged code is finished, messages need to be unwrapped:

```csharp
wrapper.Unwrap();
```

This method will send messages of type `Wrapper<TestInt>` to all subscribed
recipients.

`IRecipient<T>` interfacing is not supported.
Subscription is only supported manually. For example:

```csharp
void Receive(object _, Wrapped<TestInt> message)
{
    result = message.data;
}
messenger.Register<Wrapped<TestInt>>(recipient, Receive);
```

### Localization Asset Binding

Asset binding is a special kind of reflection binding.
Syntax works as follows:

```uxml

<ui:Image view-data-key="{#Flag>image}"/>
```

Where key must be stored in `view-data-key` attribute
and start with `#` operator.

After `#` follows table entry name.
![image](https://user-images.githubusercontent.com/30902981/219870975-7588d0f8-996f-448b-8800-d06f35443207.png)

After entry name follows `>` symbol. It is used as separator.

And then property path of current `VisualElement` is specified.

Property used in example:

```csharp
UnityEngine.UIElements.Image.image
```

### Binding Types Limitations

In order to create bindings package relies
on explicitly declared generic types.

There are 2 types of those:

1. `ISingleSolver` - provides binding support for specified type.
2. `IMultiSolver` - provides binding support for
   converting one type to another.

For example there are some built in generics
solvers implemented in package:

```csharp
    public class IntSolver : SingleSolver<int> { }
    public class UintSolver : SingleSolver<uint> { }
    public class ByteSolver : SingleSolver<byte> { }
    public class FloatSolver : SingleSolver<float> { }
    public class DoubleSolver : SingleSolver<double> { }
    public class StringSolver : SingleSolver<string> { }
    public class BoolSolver : SingleSolver<bool> { }
```

In order to support other types you need to implement solvers yourself
simply inheriting from `SingleSolver<T>`.

Some bindings rely on `IMultiSolver` and they must be implemented manually.
For example this `MultiSolver` provides a way to convert `Texture2D`
to `Texture`:

```csharp
// Converter need to be preserved in case you use code stripping
[Preserve]
public class TextureToTexture2DConverter : MultiSolver<Texture2D, Texture>
{
    protected override Texture Convert(Texture2D value) => value;
}
```

`MultiSolver` requires to implement `Convert` method in which you
specify how `Texture2D` is assigned to `Texture` setter.

`IMultiSolver` are required for **Localized Asset Binding** or when
you want to bind something that does not match types. For example `IStyle`
properties usually use very specific types which are not convenient
to work with in `ViewModel`, but can be implicitly converted.

Most of the time when solvers are not available reflection fallback
will be used with a warning. While fully functional, fallbacks are
performance heavy and generate a lot of garbage when used.