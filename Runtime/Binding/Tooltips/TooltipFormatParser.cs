using System;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine.UIElements;

namespace MVVMToolkit.Binding.Tooltips
{
    public class TooltipFormatParser : StringFormatParser
    {
        public TooltipFormatParser(INotifyPropertyChanged binding, Action<VisualElement, string> operation) : base(
            binding, operation) { }

        private readonly List<TooltipManipulator> _manipulators = new();

        public override void Process(VisualElement element, string key)
        {
            base.Process(element, key);
            var tooltip = new TooltipManipulator();
            _manipulators.Add(tooltip);
            element.AddManipulator(tooltip);
        }

        public override void Dispose()
        {
            base.Dispose();
            foreach (var manipulator in _manipulators)
            {
                VisualElementExtensions.RemoveManipulator(null, manipulator);
                manipulator.Dispose();
            }
        }
    }
}