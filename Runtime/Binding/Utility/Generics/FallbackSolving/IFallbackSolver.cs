using System;
using System.Reflection;

namespace MVVMToolkit.Binding.Generics
{
    internal interface IFallbackSolver
    {
        /// <summary>
        /// Creates delegate for getting value from first object and setting it to second object
        /// using delegates.
        /// </summary>
        /// <param name="get">MethodInfo for getting value</param>
        /// <param name="set">MethodInfo for settings value</param>
        /// <returns>Action that stores value from get and uses it as parameter to set methods</returns>
        Action<object, object> Solve(MethodInfo get, MethodInfo set);

        Action Solve(MethodInfo get, object source, MethodInfo set, object target);
    }
}