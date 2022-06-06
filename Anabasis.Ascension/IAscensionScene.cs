using Anabasis.Core;
using Anabasis.Tasks;

namespace Anabasis.Ascension;

public interface IAscensionScene : IAnabasisContext
{
    public AnabasisTask LoadAsync(IProgress<SceneLoadStatus> progress);
    AnabasisTask IAnabasisContext.LoadAsync() => LoadAsync(new AnabasisProgress<SceneLoadStatus>());
}