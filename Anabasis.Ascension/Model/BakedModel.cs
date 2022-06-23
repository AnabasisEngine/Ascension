using Anabasis.Core.Graphics.Buffers;
using Anabasis.Core.Graphics.Rendering;
using Silk.NET.OpenGL;
using VertexArray = Anabasis.Core.Graphics.Rendering.VertexArray;

namespace Anabasis.Ascension.Model;

public sealed class BakedModel : IDisposable
{
    private readonly BakedScene     _scene;
    private readonly GraphicsBuffer _buffer;
    private readonly int            _matIdx;
    private readonly uint            _idxCount;
    private readonly uint            _idxOffs;

    internal BakedModel(BakedScene scene, BakedScene.UnbakedModel dough) {
        _scene = scene;
        _buffer = dough.Buffer;
        _matIdx = dough.MaterialIndex;
        _idxCount = dough.IndexCount;
        _idxOffs = dough.IndexOffset;
    }

    public void BindMaterial(BufferTargetARB target, uint index) => _scene.BindMaterial(_matIdx, target, index);

    public void BindBuffers(VertexArray vertexArray, out uint count, out uint indexOffset) {
        vertexArray.FormatAndBindVertexBuffer<AscensionBasicVertexData>(_buffer);
        vertexArray.BindIndexBuffer(_buffer.Typed<uint>());
        count = _idxCount;
        indexOffset = _idxOffs;
    }

    public void Dispose() {
        _buffer.Dispose();
    }
}