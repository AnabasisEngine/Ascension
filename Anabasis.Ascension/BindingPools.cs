namespace Anabasis.Ascension;

public sealed class BindingPools
{
    public readonly BindUnitPool TextureBindingPool       = new();
    public readonly BindUnitPool UniformBufferBindingPool = new();
}