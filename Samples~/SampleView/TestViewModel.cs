using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MVVMToolkit;
using UnityEngine;
using UnityEngine.UIElements;


// ViewModels inherit from ViewModel class which is a MonoBehavior class.
public partial class TestViewModel : ViewModel
{
    // To bind a simple property, just create a backing field
    // and attach [ObservableProperty] attribute. TestInt property will be generated.
    [ObservableProperty] private int _testInt = 12;

    // To bind a method to click event you will need ICommand property.
    // [RelayCommand] will automatically generate it for you.
    [RelayCommand]
    private void Increment() => Counter++;

    // In some scenarios multiple properties can be attached to one backing field.
    // In this case use [NotifyPropertyChangedFor] attribute
    [ObservableProperty, NotifyPropertyChangedFor(nameof(FontSize))]
    private int _counter;

    // This property provides a proper type for VisualElement.style.fontSize for binding
    public StyleLength FontSize => Counter;

    // You can also nest groups. Groups must implement INotifyPropertyChanged interface
    [ObservableProperty] private NestedGroup _group = new();

    // In OnInit override we can do manual changes to ViewModel
    protected override void OnInit()
    {
        Group.NestedFloat = 24f;
        Group.NestedBool = true;
        Group.Deep = new();
    }

    // We can also change nested groups and all changes will be reflected in bindings
    // But keep in mind that this operation will cause all bindings of group to rebind from scratch
    [RelayCommand]
    private void RandomizeNested()
    {
        Group = new()
        {
            NestedFloat = Random.Range(0, 5f),
            Deep = new()
            {
                NestedFloat = Random.Range(500, 1000f)
            }
        };
    }
}

// You can use ObservableObject as a base or [ObservableObject] property to use CommunityToolkit.Mvvm codegen
// in nested groups
public partial class NestedGroup : ObservableObject
{
    [ObservableProperty] private float _nestedFloat = 3f;
    [ObservableProperty] private bool _nestedBool;
    [ObservableProperty] private NestedGroup _deep;
}