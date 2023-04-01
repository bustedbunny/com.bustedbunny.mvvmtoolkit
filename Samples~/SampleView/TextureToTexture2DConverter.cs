using MVVMToolkit.Binding.Generics;
using UnityEngine;
using UnityEngine.Scripting;

// Converter need to be preserved in case you use code stripping
namespace SampleView
{
    [Preserve]
    public class TextureToTexture2DConverter : MultiSolver<Texture2D, Texture>
    {
        protected override Texture Convert(Texture2D value) => value;
    }
}