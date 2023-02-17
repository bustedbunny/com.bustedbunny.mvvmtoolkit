using UnityEngine;
using UnityEngine.UIElements;

namespace MVVMToolkit.Binding.Tooltips
{
    public class TooltipElement : VisualElement
    {
        public const string TooltipContainerStyle = "MVVMTK-tooltip-container";
        public const string TooltipLabelStyle = "MVVMTK-tooltip-label";

        public TooltipElement()
        {
            style.position = Position.Absolute;
            _tooltipLabel = new();
            Add(_tooltipLabel);

            AddToClassList(TooltipContainerStyle);
            _tooltipLabel.AddToClassList(TooltipLabelStyle);
        }

        private readonly Label _tooltipLabel;

        public string Text
        {
            set => _tooltipLabel.text = value;
        }


        public void SetPosition(Vector2 pos)
        {
            style.left = new Length(pos.x, LengthUnit.Pixel);
            style.top = new Length(pos.y, LengthUnit.Pixel);
        }
    }
}