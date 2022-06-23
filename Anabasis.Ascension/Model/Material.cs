using System.Numerics;
using System.Runtime.InteropServices;

namespace Anabasis.Ascension.Model;

// Uniform buffer is aligned on 16-byte increments in std140 mode, requires reflection otherwise
[StructLayout(LayoutKind.Sequential, Size = 48)]
public struct Material
{
    public Vector4 DiffuseAlbedo;
    public Vector4 ReflectionFactor;
    public float   Shininess;
}