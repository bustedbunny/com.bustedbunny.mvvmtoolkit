using System;

namespace MVVMToolkit.Binding.Generics
{
    public class ConversionSolver<T0, T1> : IConversionSolver
    {
        public bool CanSolve(Type first, Type second)
        {
            if (typeof(T0) == first && typeof(T1) == second) return true;
            return typeof(T1) == first && typeof(T0) == second;
        }
    }
}