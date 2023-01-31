using CommunityToolkit.Mvvm.Messaging;
using MVVMToolkit;
using UnityEngine;
using UnityEngine.UIElements;

// Views inherit from BaseView or EmbeddedView (if your view must be part of another)
// We also inherit from IRecipient interface. This will be automatically subscribed to Messenger
public class TestView : BaseView, IRecipient<OpenTestViewMessage>
{
    // On message receive we will enable our View 
    public void Receive(OpenTestViewMessage message)
    {
        enabled = true;
    }

    // You can override OnInit to make manual changes to hierarchy
    protected override void OnInit()
    {
        RootVisualElement.style.backgroundColor = new StyleColor(Color.gray);
    }
}

public class OpenTestViewMessage { }