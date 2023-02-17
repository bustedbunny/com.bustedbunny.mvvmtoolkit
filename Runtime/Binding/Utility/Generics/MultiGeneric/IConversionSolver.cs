using System;

namespace MVVMToolkit.Binding.Generics
{
    public interface IConversionSolver
    {
        public bool CanSolve(Type first, Type second);
    }
}