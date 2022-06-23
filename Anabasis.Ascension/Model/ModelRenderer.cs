using Anabasis.Core;
using Anabasis.Core.Graphics;
using Anabasis.Core.Graphics.Shaders;
using Silk.NET.OpenGL;
using VertexArray = Anabasis.Core.Graphics.Rendering.VertexArray;

namespace Anabasis.Ascension.Model;

public sealed class ModelRenderer : IDisposable
{
    private readonly GL             _gl;
    private readonly BindingPools   _bindingPools;
    private readonly IShaderPackage _package;
    private readonly ShaderProgram  _fragmentProgram;
    private readonly VertexArray    _array;

    public ModelRenderer(GL gl, BindingPools bindingPools, VertexArray array, IShaderPackage package,
        ShaderProgram fragmentProgram) {
        _gl = gl;
        _bindingPools = bindingPools;
        _package = package;
        _fragmentProgram = fragmentProgram;
        _array = array;
    }

    public void DrawModel(BakedModel model) {
        if (!_bindingPools.UniformBufferBindingPool.TryTake(out int matBinding))
            throw new InvalidOperationException("Could not reserve binding for material uniforms");

        model.BindMaterial(BufferTargetARB.UniformBuffer, (uint)matBinding);
        uint? i = null;
        _fragmentProgram.SetUniformBlockBinding("Material", ref i, (uint)matBinding);
        model.BindBuffers(_array, out uint idxCount, out uint idxOffs);
        _package.Use();
        _gl.DrawElementsI(PrimitiveType.Triangles, idxCount, DrawElementsType.UnsignedInt, idxOffs);
    }

    public void Dispose() {
        _array.Dispose();
    }
}