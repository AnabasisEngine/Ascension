using System.Collections.Immutable;
using System.Runtime.InteropServices;
using Anabasis.Core;
using Anabasis.Core.Graphics.Buffers;
using Microsoft.CodeAnalysis.PooledObjects;
using Silk.NET.OpenGL;

namespace Anabasis.Ascension.Model;

public sealed class BakedScene : IDisposable
{
    private readonly GraphicsBuffer             _materialsBuffer;
    public readonly  ImmutableArray<BakedModel> Models;

    internal BakedScene(GL gl, GraphicsBuffer materialsBuffer, ImmutableArray<UnbakedModel> models) {
        _materialsBuffer = materialsBuffer;
        ArrayBuilder<BakedModel> builder = ArrayBuilder<BakedModel>.GetInstance(models.Length);
        foreach (UnbakedModel unbakedModel in models) {
            builder.Add(new BakedModel(this, unbakedModel));
        }

        Models = builder.ToImmutableAndFree();
    }

    public void Dispose() {
        _materialsBuffer.Dispose();
        foreach (BakedModel model in Models) {
            model.Dispose();
        }
    }

    public void BindMaterial(int material, BufferTargetARB target, uint index) {
        int size = Marshal.SizeOf<Material>();
        _materialsBuffer.BindRange(target, index, material * size, (uint)size);
    }

    internal record UnbakedModel(GraphicsBuffer Buffer, int MaterialIndex, uint IndexCount, uint IndexOffset);
}