using System.ComponentModel;
using MVVMToolkit.Binding.Localization.Source;
using UnityEngine.Localization;

namespace MVVMToolkit.Binding.Localization
{
    public static class LocalizationBindingUtility
    {
        public static void BindContext(this LocalizedString ls, INotifyPropertyChanged bindingContext)
        {
            var unused = new BindingGroup(bindingContext, ls);
        }

        public static void UnbindAllContext(this LocalizedString ls)
        {
            if (ls.Arguments is not null)
            {
                for (var ind = ls.Arguments.Count - 1; ind >= 0; ind--)
                {
                    var argument = ls.Arguments[ind];
                    if (argument is BindingGroup binding)
                    {
                        binding.Dispose();
                        ls.Arguments.RemoveAt(ind);
                    }
                }
            }
        }
    }
}