using UnityEngine.Scripting;

namespace MVVMToolkit.Binding.Generics
{
    [Preserve]
    public class IntSolver : SingleSolver<int> { }

    [Preserve]
    public class UintSolver : SingleSolver<uint> { }

    [Preserve]
    public class ByteSolver : SingleSolver<byte> { }

    [Preserve]
    public class FloatSolver : SingleSolver<float> { }

    [Preserve]
    public class DoubleSolver : SingleSolver<double> { }

    [Preserve]
    public class StringSolver : SingleSolver<string> { }

    [Preserve]
    public class BoolSolver : SingleSolver<bool> { }
}