using CommunityToolkit.Mvvm.ComponentModel;

namespace MVVMToolkit
{
    [ObservableObject]
    public abstract partial class ViewModel : UIObject
    {
        protected virtual void OnDestroy()
        {
            Messenger?.UnregisterAll(this);
        }
    }
}