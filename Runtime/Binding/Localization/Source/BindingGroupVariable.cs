using System;

namespace MVVMToolkit.Binding.Localization.Source
{
    public class BindingGroupVariable : LocalizationVariable<BindingGroup>
    {
        private BindingGroup _prevValue;

        public BindingGroupVariable(Func<BindingGroup> getter) : base(getter)
        {
            ValueChanged += variable =>
            {
                var newValue = (BindingGroup)variable.GetSourceValue(null);
                if (_prevValue == newValue) return;
                _prevValue?.Dispose();
                _prevValue = newValue;
            };
        }

        public void Clear()
        {
            _prevValue?.Dispose();
        }
    }
}