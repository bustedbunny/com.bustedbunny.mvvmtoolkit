using System.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using UnityEngine.Assertions;

namespace MVVMToolkit.Binding
{
    public static class CommandUtility
    {
        public static void Parse(INotifyPropertyChanged bindingContext, string key, out IRelayCommand command,
            out object argument)
        {
            Assert.IsNotNull(bindingContext);
            Throw.ThrowNullOrEmpty(key);

            var keys = key.Split(':');

            Assert.IsTrue(keys.Length is <= 2 and > 0);

            var commandPath = keys[0];
            FormatCommandPath(ref commandPath);
            command = GetCommand(bindingContext, commandPath);

            Assert.IsNotNull(command);

            argument = keys.Length == 2 ? GetArgument(keys[1]) : null;
        }

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
            ParsingUtility.GetTargetObject(source, prompt, out var target, out var propertyName);
            var property = PropertyUtility.GetGetProperty(target, propertyName);
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