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
            var lastIndex = prompt.LastIndexOf('.');
            if (lastIndex != -1)
            {
                lastIndex++;

                var substring = prompt[lastIndex..];


                if (substring.StartsWith(prefix))
                {
                    substring = substring[2..];

                    prompt = prompt[..lastIndex] + substring;
                }
            }
            else
            {
                if (prompt.StartsWith(prefix))
                {
                    prompt = prompt[2..];
                }
            }
        }

        public static IRelayCommand GetCommand(object source, string prompt)
        {
            FormatCommandPath(ref prompt);
            var property = PropertyUtility.GetGetProperty(source, prompt);
            return property.GetValue(source) as IRelayCommand;
        }

        public static object GetArgument(string key)
        {
            if (bool.TryParse(key, out var boolResult)) return boolResult;
            if (int.TryParse(key, out var intResult)) return intResult;
            if (float.TryParse(key, out var floatResult)) return floatResult;
            return key;
        }
    }
}