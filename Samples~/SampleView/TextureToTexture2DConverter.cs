using System;
using MVVMToolkit.Binding.Generics;
using UnityEngine;

public class TextureToTexture2DConverter : ConversionSolver<Texture, Texture2D>
{
    protected override void AssignValue(Action<Texture> setter, Texture2D value)
    {
        setter(value);
    }
}