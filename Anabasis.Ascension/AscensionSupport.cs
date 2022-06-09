using Silk.NET.OpenGL;

namespace Anabasis.Ascension;

/// <summary>
/// Opaque type containing support services used internally by <see cref="AscensionGame"/>
/// which should not be exposed to inheritors
/// </summary>
public sealed class AscensionSupport
{
    public AscensionSupport(IServiceProvider serviceProvider) {
        ServiceProvider = serviceProvider;
    }
    internal IServiceProvider ServiceProvider { get; }
}