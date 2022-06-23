using System.Numerics;
using Anabasis.Core.Graphics.Textures;
using Silk.NET.Maths;

namespace Anabasis.Ascension;

public static class TextureExtensions
{
    /// <summary>
    /// Constructs the vector <c>(1f / Width, 1f / Height)</c> for this texture view.
    /// <remarks>The resulting vector can be used to normalize texture coords in pixel space to the range [0,1]</remarks> 
    /// </summary>
    public static Vector2 TexelCoeffs(this Texture2D texture) => new(1f / texture.Width, 1f / texture.Height);

    /// <summary>
    /// Normalizes the given pixel-space texture coordinates to the range [0,1f] for use in vertex attributes for
    /// shader programs
    /// </summary>
    public static Vector2 NormalizeTexCoordFloat(this Texture2D texture, Vector2D<ushort> coords) =>
        coords.As<float>().ToSystem() * texture.TexelCoeffs();

    /// <summary>
    /// Normalizes the given pixel-space texture coordinates to the range [0,MAX_USHORT] for storage
    /// efficiency in vertex attributes for shader programs.
    /// </summary>
    public static Vector2D<ushort> NormalizeTexCoordUshort(this Texture2D texture, Vector2D<ushort> coords) =>
        (texture.NormalizeTexCoordFloat(coords) * ushort.MaxValue).ToGeneric().As<ushort>();
}