using UnityEngine.UIElements;

namespace MVVMToolkit.Binding.Tooltips
{
    public static class TooltipUtility
    {
        public static void TooltipBindingOperation(VisualElement element, string s)
        {
            using var pooled = TooltipChangedEvent.GetPooled();
            element.tooltip = s;
            pooled.target = element;
            pooled.tooltip = s;
            element.SendEvent(pooled);
        }
    }
}