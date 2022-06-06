using Anabasis.Core;
using Anabasis.Tasks;
using Silk.NET.OpenGL;

namespace Anabasis.Ascension;

public abstract class AscensionGame : AnabasisGame
{
    private IAnabasisContext? _currentScene;
    
    protected AscensionGame(GL gl) {
        Gl = gl;
    }
    protected GL Gl { get; }

    protected IAnabasisContext CurrentScene {
        get => _currentScene!;
        set {
            // Unload previous scene if relevant
            if (_currentScene is IAnabasisTaskAsyncDisposable d)
                d.DisposeAsync().Forget();
            else if (_currentScene is IDisposable s) 
                s.Dispose();
            
            // Set new scene and load
            _currentScene = value;
            if (value is IAscensionScene scene) {
                SceneLoadTask = scene.LoadAsync(new AnabasisProgress<SceneLoadStatus>(s => SceneLoadStatus = s))
                    .AsTask();
            } else {
                SceneLoadTask = value.LoadAsync().AsTask();
            }
        }
    }

    protected Task SceneLoadTask { get; private set; } = Task.CompletedTask;
    protected SceneLoadStatus SceneLoadStatus { get; private set; } = new();

    protected abstract AnabasisTask<IAnabasisContext> CreateInitialSceneAsync();

    public override async AnabasisTask LoadAsync() {
        CurrentScene = await CreateInitialSceneAsync();
    }

    public override void Update() {
        if(!LoadTask.IsCompleted)
            return;
        if (SceneLoadTask.IsCompleted) {
            SceneLoadTask.GetAwaiter().GetResult();
            CurrentScene.Update();
        } else
            UpdateLoading();
    }

    protected virtual void UpdateLoading() {
    }

    public override void Render() {
        if(!LoadTask.IsCompleted)
            return;
        if (SceneLoadTask.IsCompleted) {
            SceneLoadTask.GetAwaiter().GetResult();
            CurrentScene.Render();
        }
        else
            RenderLoading();
    }

    protected virtual void RenderLoading() {
    }
}