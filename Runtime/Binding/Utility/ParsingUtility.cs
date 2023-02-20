using System;
using System.Diagnostics;

namespace MVVMToolkit.Binding
{
    public static class ParsingUtility
    {
        public static string[] GetFormatKeys(string str)
        {
            var count = 0;
            // -2 is because after opening `{` there needs to be at least 1 char for key, and 1 char to close braces.
            for (var i = 0; i < str.Length - 2; i++)
            {
                var chr = str[i];
                if (chr is '{') count++;
            }

            if (count > 0)
            {
                var resultInd = 0;
                var result = new string[count];
                for (var i = 0; i < str.Length - 2; i++)
                {
                    var chr = str[i];
                    if (chr is '{')
                    {
                        i++;
                        var substr = str[i..];
                        var ind = substr.IndexOf('}');

                        if (ind <= 0)
                        {
                            throw new StringParsingException(
                                $"Invalid format provided: {str}. No enclosing brace was found.");
                        }

                        result[resultInd] = substr[..ind];
                        resultInd++;
                        i += ind;
                    }
                }

                return result;
            }

            return null;
        }

        public class StringParsingException : Exception
        {
            public StringParsingException(string message) : base(message) { }
        }

        public static void GetTargetObject(object root, string key, out object target, out string propertyName)
        {
            Throw.ThrowNullOrEmpty(key);
            target = root;
            var paths = key.Split('.');
            if (paths.Length == 1)
            {
                propertyName = paths[0];
                return;
            }

            for (int i = 0; i < paths.Length - 1; i++)
            {
                var path = paths[i];
                var type = target.GetType();
                var nestedProperty = PropertyUtility.GetGetProperty(type, path);
                // if returned value is interface, we should obtain next property as interface
                if (nestedProperty.PropertyType.IsInterface)
                {
                    paths[i + 1] = $"{nestedProperty.PropertyType.Name}." + paths[i + 1];
                }

                target = nestedProperty.GetValue(target);
                Debug.Assert(root != null, $"Obtained nested object is null in type {type.Name}");
            }

            propertyName = paths[^1];
        }
    }
}