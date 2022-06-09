using Anabasis.Ascension.Rendering;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Anabasis.Ascension;

public static class AscensionServiceCollectionExtensions
{
    public static void AddAscension(this IServiceCollection collection) {
        collection.TryAddScoped<AscensionSupport>();
        collection.TryAddScoped<Camera2D>();
        collection.TryAddScoped<AscensionRenderResources>();
        collection.TryAddScoped<BindingPools>();
    }
}