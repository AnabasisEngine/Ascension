using System.Numerics;

namespace Anabasis.Ascension;

/// <summary>
/// Helper utility for creating model position matrices
/// </summary>
public static class Transforms
{
    /// <summary>
    /// Flags indicating reflections around some axes
    /// </summary>
    [Flags]
    public enum Reflections
    {
        None       = 0,
        Vertical   = 1,
        Horizontal = 2,
        Depth      = 4,
    }

    public static int GetXScale(this Reflections reflections) => (reflections & Reflections.Horizontal) != 0 ? -1 : 1;
    public static int GetYScale(this Reflections reflections) => (reflections & Reflections.Vertical) != 0 ? -1 : 1;
    public static int GetZScale(this Reflections reflections) => (reflections & Reflections.Depth) != 0 ? -1 : 1;

    /// <summary>
    /// Creates a model matrix for rendering a 2d sprite
    /// </summary>
    /// <param name="position">The world-position of the sprite</param>
    /// <param name="rotation">The amount to rotate the sprite, in radians</param>
    /// <param name="flip">What, if any, reflections to perform</param>
    public static Matrix4x4 CreateModelMatrix(Vector3 position, float rotation, Reflections flip = Reflections.None) =>
        Matrix4x4.CreateScale(flip.GetXScale(), flip.GetYScale(), flip.GetZScale()) *
        (Matrix4x4.CreateTranslation(position) * Matrix4x4.CreateRotationZ(rotation));
}