using System.Runtime.InteropServices;
using Anabasis.Core.Buffers;
using Anabasis.Tasks;
using Silk.NET.OpenGL;

namespace Anabasis.Ascension.Rendering;

public sealed class AscensionRenderResources : IDisposable
{
    private readonly BindingPools _pools;

    public AscensionRenderResources(GL gl, BindingPools pools) {
        _pools = pools;
        UniformBuffer = new GraphicsBuffer(gl, BufferTargetARB.UniformBuffer);
        UniformBuffer.AllocateBuffer(Marshal.SizeOf<StandardTransformUniforms>());
        if (!pools.UniformBufferBindingPool.TryTake(out _binding))
            throw new InvalidOperationException();
        AscensionVertexShader.CreateAsync(gl)
            .ContinueWith(p => {
                VertexShader = p;
                UniformBuffer.BindIndex(BufferTargetARB.UniformBuffer, (uint)_binding);
                VertexShader.BindTransformsUniformBuffer((uint)_binding);
            })
            .Forget();
    }
    public GraphicsBuffer UniformBuffer { get; }
    public AscensionVertexShader VertexShader { get; private set; } = null!;

    private          StandardTransformUniforms _transforms;
    private readonly int                       _binding;

    public StandardTransformUniforms Transforms {
        get => _transforms;
        set {
            if(_transforms.Equals(value))
                return;
            _transforms = value;
            UniformBuffer.Typed<StandardTransformUniforms>().Write(MemoryMarshal.CreateReadOnlySpan(ref _transforms, 1));
        }
    }

    public void Dispose() {
        UniformBuffer.Dispose();
        VertexShader.Dispose();
        _pools.UniformBufferBindingPool.Return(_binding);
    }
}