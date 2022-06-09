using System.Numerics;
using System.Runtime.InteropServices;
using Anabasis.Core;
using Anabasis.Core.Rendering;
using Silk.NET.OpenGL;

namespace Anabasis.Ascension;

[VertexType]
[StructLayout(LayoutKind.Sequential)]
public partial struct AscensionBasicVertexData
{
    [VertexAttribute("vPos", AttributeType.FloatVec3, 0)]
    public Vector3 Position;

    [VertexAttribute("vColor", AttributeType.FloatVec4, 1, true)]
    public Color Color;

    [VertexAttribute("vUv", AttributeType.FloatVec2, 2)]
    public Vector2 TexCoord;
}