using UnityEngine.UIElements;

namespace MVVMToolkit.Binding.Tooltips
{
    public class TooltipChangedEvent : EventBase<TooltipChangedEvent>
    {
        public string tooltip;
    }
}