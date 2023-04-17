using CommunityToolkit.Mvvm.Input;

namespace MVVMToolkit.Binding
{
    public static class CommandUtility
    {
        public static void FormatCommandPath(ref string prompt)
        {
            const string suffix = "Command";
            if (!prompt.EndsWith(suffix))
            {
                prompt += suffix;
            }

            const string prefix = "On";
            if (prompt.StartsWith(prefix))
            {
                prompt = prompt[2..];
            }
        }

        public static IRelayCommand GetCommand(object source, string prompt)
        {
            FormatCommandPath(ref prompt);
            var property = PropertyUtility.GetGetProperty(source, prompt);
            return property.GetValue(source) as IRelayCommand;
        }
    }
}