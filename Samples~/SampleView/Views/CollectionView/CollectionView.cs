using CommunityToolkit.Mvvm.Messaging;
using MVVMToolkit;

namespace SampleView.CollectionView
{
    public class CollectionViewView : BaseView, IRecipient<OpenCollectionViewMessage>
    {
        public void Receive(OpenCollectionViewMessage message)
        {
            enabled = true;
        }
    }

    public class OpenCollectionViewMessage { }
}