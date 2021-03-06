using System.Numerics;
using System.Runtime.InteropServices;

namespace Anabasis.Ascension;

[StructLayout(LayoutKind.Explicit, Size = 172)]
public struct StandardTransformUniforms
    : IEquatable<StandardTransformUniforms>
{
    [FieldOffset(0)]
    public Matrix4x4 Model;

    [FieldOffset(64)]
    public Matrix4x4 World;

    [FieldOffset(128)]
    public Matrix4x4 Projection;

    public bool Equals(StandardTransformUniforms other) => Model.Equals(other.Model) && World.Equals(other.World) &&
                                                           Projection.Equals(other.Projection);

    public override bool Equals(object? obj) => obj is StandardTransformUniforms other && Equals(other);

    public override int GetHashCode() => HashCode.Combine(Model, World, Projection);

    public static bool operator ==(StandardTransformUniforms left, StandardTransformUniforms right) =>
        left.Equals(right);

    public static bool operator !=(StandardTransformUniforms left, StandardTransformUniforms right) =>
        !left.Equals(right);
}