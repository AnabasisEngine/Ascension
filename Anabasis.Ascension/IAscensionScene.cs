using Anabasis.Core;
using Anabasis.Tasks;

namespace Anabasis.Ascension;

/// <summary>
/// Provides an <see cref="IProgress{SceneLoadStatus}"/> overload for <see cref="IAnabasisContext.LoadAsync"/> to
/// support easier loading screens.
/// </summary>
public interface IAscensionScene : IAnabasisContext
{
    public AnabasisTask LoadAsync(IProgress<SceneLoadStatus> progress);
    AnabasisTask IAnabasisContext.LoadAsync() => LoadAsync(new AnabasisProgress<SceneLoadStatus>());
}