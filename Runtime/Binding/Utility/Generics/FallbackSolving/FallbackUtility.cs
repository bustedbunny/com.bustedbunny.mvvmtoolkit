using System;
using System.Reflection;

namespace MVVMToolkit.Binding.Generics
{
    internal static class FallbackUtility
    {
        public static IFallbackSolver CreateSolver(Type source, Type getType, Type target, Type setType)
        {
            Type genericType;
            if (getType == setType)
            {
                genericType = typeof(SingleFallback<,,>).MakeGenericType(source, target, getType);
            }
            else if (getType.IsAssignableFrom(setType))
            {
                genericType = typeof(AssignableFallback<,,,>).MakeGenericType(source, getType, target, setType);
            }
            else
            {
                genericType = typeof(ConversionFallback<,,,>).MakeGenericType(source, getType, target, setType);
            }

            return (IFallbackSolver)Activator.CreateInstance(genericType);
        }


        private const string Implicit = "op_Implicit";
        private const string Explicit = "op_Explicit";
        private const BindingFlags Flags = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public;

        internal static Func<TFrom, TTo> GetConverter<TFrom, TTo>()
        {
            Type[] getTypeArray = { typeof(TFrom) };
            var converter = typeof(TFrom).GetMethod(Implicit, Flags, null, getTypeArray, null);

            if (converter is null || !IsReturnType<TTo>(converter))
            {
                converter = typeof(TFrom).GetMethod(Explicit, Flags, null, getTypeArray, null);
            }

            if (converter is null || !IsReturnType<TTo>(converter))
            {
                converter = typeof(TTo).GetMethod(Implicit, Flags, null, getTypeArray, null);
            }

            if (converter is null || !IsReturnType<TTo>(converter))
            {
                converter ??= typeof(TTo).GetMethod(Explicit, Flags, null, getTypeArray, null);
            }

            if (converter is null || !IsReturnType<TTo>(converter))
            {
                converter = Fallback<TFrom, TFrom, TTo>();
                converter ??= Fallback<TTo, TFrom, TTo>();
            }

            if (converter is null)
            {
                throw new BindingException(
                    $"No implicit or explicit converter found for conversion from {nameof(TFrom)} to {nameof(TTo)}");
            }

            return (Func<TFrom, TTo>)converter.CreateDelegate(typeof(Func<TFrom, TTo>));
        }

        private static MethodInfo Fallback<TImplicitSource, TFrom, TTo>()
        {
            MethodInfo first = null;
            foreach (var x in typeof(TImplicitSource).GetMethods())
            {
                if (x.Name is Implicit or Explicit && x.ReturnType == typeof(TTo))
                {
                    var parameters = x.GetParameters();
                    if (parameters.Length != 1)
                    {
                        continue;
                    }

                    if (parameters[0].ParameterType != typeof(TFrom) || !x.IsSpecialName)
                    {
                        continue;
                    }

                    first = x;
                    break;
                }
            }

            return first;
        }

        private static bool IsReturnType<TTo>(MethodInfo mi)
        {
            return mi.ReturnType == typeof(TTo);
        }
    }
}