using System.Buffers;
using System.Runtime.InteropServices;
using Anabasis.Core.Graphics.Buffers;
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
        _memoryOwner = UniformBuffer.MapSlice<StandardTransformUniforms>(0, UniformBuffer.Length);
    }
    public GraphicsBuffer UniformBuffer { get; }
    public AscensionVertexShader VertexShader { get; private set; } = null!;

    private readonly int                                     _binding;
    private readonly IMemoryOwner<StandardTransformUniforms> _memoryOwner;

    public ref StandardTransformUniforms Transforms => ref _memoryOwner.Memory.Span[0];

    public void Dispose() {
        _memoryOwner.Dispose();
        UniformBuffer.Dispose();
        VertexShader.Dispose();
        _pools.UniformBufferBindingPool.Return(_binding);
    }
}