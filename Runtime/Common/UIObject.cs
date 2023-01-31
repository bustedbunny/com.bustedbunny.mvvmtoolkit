using CommunityToolkit.Mvvm.Messaging;
using UnityEngine;

namespace MVVMToolkit
{
    public class UIObject : MonoBehaviour
    {
        protected StrongReferenceMessenger Messenger { get; private set; }
        protected ServiceProvider ServiceProvider { get; private set; }

        public void Attach(StrongReferenceMessenger messenger, ServiceProvider serviceProvider)
        {
            Messenger = messenger;
            ServiceProvider = serviceProvider;
            Messenger.RegisterAll(this);

            OnInit();
        }

        protected virtual void OnInit() { }
    }
}