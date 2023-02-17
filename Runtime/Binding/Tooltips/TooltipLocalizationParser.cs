using System;
using System.Collections.Generic;
using System.ComponentModel;
using MVVMToolkit.Binding.Localization;
using UnityEngine.Localization;
using UnityEngine.UIElements;

namespace MVVMToolkit.Binding.Tooltips
{
    public class TooltipLocalizationParser : LocalizationTextParser
    {
        public TooltipLocalizationParser(INotifyPropertyChanged viewModel, LocalizedStringTable[] stringTables,
            Action<VisualElement, string> bindingOperation) : base(viewModel, stringTables, bindingOperation) { }

        private readonly List<TooltipManipulator> _manipulators = new();

        public override void Process(VisualElement element, string key)
        {
            base.Process(element, key);
            element.tooltip = key;
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