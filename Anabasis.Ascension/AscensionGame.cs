using System.Diagnostics.CodeAnalysis;
using Anabasis.Core;
using Anabasis.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Silk.NET.OpenGL;

namespace Anabasis.Ascension;

public abstract class AscensionGame : AnabasisGame
{
    private readonly AscensionSupport  _support;
    private          IAnabasisContext? _currentScene;

    protected AscensionGame(GL gl, AscensionSupport support) {
        _support = support;
        Gl = gl;
    }

    protected GL Gl { get; }

    protected IAnabasisContext CurrentScene {
        get => _currentScene!;
        set {
            // Unload previous scene if relevant
            switch (_currentScene) {
                case IAnabasisTaskAsyncDisposable d:
                    d.DisposeAsync().Forget();
                    break;
                case IDisposable s:
                    s.Dispose();
                    break;
            }

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

    protected T CreateScene<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] T>(params object[] parameters)
        where T : IAnabasisContext => ActivatorUtilities.CreateInstance<T>(_support.ServiceProvider, parameters);

    protected Task SceneLoadTask { get; private set; } = Task.CompletedTask;
    protected SceneLoadStatus SceneLoadStatus { get; private set; } = new();

    protected abstract AnabasisTask<IAnabasisContext> CreateInitialSceneAsync();

    public override async AnabasisTask LoadAsync() {
        CurrentScene = await CreateInitialSceneAsync();
    }

    public override void Update() {
        if (!LoadTask.IsCompleted)
            return;
        if (SceneLoadTask.IsCompleted) {
            SceneLoadTask.GetAwaiter().GetResult();
            CurrentScene.Update();
        } else
            UpdateLoading();
    }

    protected virtual void UpdateLoading() { }

    public override void Render() {
        if (!LoadTask.IsCompleted)
            return;
        if (SceneLoadTask.IsCompleted) {
            SceneLoadTask.GetAwaiter().GetResult();
            CurrentScene.Render();
        } else
            RenderLoading();
    }

    protected virtual void RenderLoading() { }
}