using System.Buffers;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Anabasis.Ascension.Model;
using Anabasis.Core;
using Anabasis.Core.Graphics.Buffers;
using Microsoft.CodeAnalysis.PooledObjects;
using Silk.NET.Assimp;
using Silk.NET.OpenGL;
using Material = Anabasis.Ascension.Model.Material;

namespace Anabasis.Ascension.Assimp;

using Ass = Silk.NET.Assimp.Assimp;
using SMaterial = Silk.NET.Assimp.Material;

public sealed class ModelBakery : IDisposable
{
    private readonly GL  _gl;
    private readonly Ass _assimp;

    public ModelBakery(GL gl) {
        _gl = gl;
        _assimp = Ass.GetApi();
    }

    private static Vector2 ToVector2(Vector3 vec) => Unsafe.As<Vector3, Vector2>(ref vec);

    private static void AllocateMeshBuffer(in Mesh mesh, GraphicsBuffer buffer, out uint indexOffset,
        out uint vertexOffset, out uint indexLength, out uint vertexLength, out uint indices) {
        PreLoadSizes(mesh, out uint verts, out indices);
        indexLength = indices * sizeof(uint);
        vertexLength = verts * (uint)Marshal.SizeOf<AscensionBasicVertexData>();

        uint indLenAligned = MiscMath.Align(indexLength, buffer.UniformBufferOffsetAlignment);
        uint vertLenAligned = MiscMath.Align(vertexLength, buffer.UniformBufferOffsetAlignment);

        indexOffset = vertLenAligned;
        vertexOffset = 0;

        buffer.AllocateBuffer((int)(indLenAligned + vertLenAligned));
    }

    internal unsafe BakedScene LoadScene(in Scene scene) {
        GraphicsBuffer matsBuffer = new(_gl, BufferTargetARB.UniformBuffer);
        matsBuffer.AllocateBuffer((int)(scene.MNumMaterials * Marshal.SizeOf<Material>()));
        using (IMemoryOwner<Material> owner = matsBuffer.MapSlice<Material>(0, matsBuffer.Length)) {
            LoadMaterials(scene, owner.Memory.Span);
        }
        ArrayBuilder<BakedScene.UnbakedModel> models = ArrayBuilder<BakedScene.UnbakedModel>.GetInstance((int)scene.MNumMeshes);
        for (int i = 0; i < scene.MNumMeshes; i++) {
            models.Add(LoadMesh(*scene.MMeshes[i], i));
        }

        return new BakedScene(_gl, matsBuffer, models.ToImmutableAndFree());
    }

    private BakedScene.UnbakedModel LoadMesh(in Mesh mesh, int meshIdx) {
        GraphicsBuffer buffer = new(_gl);
        LoadMeshBuffer(mesh, meshIdx, buffer, out uint idxOffs, out uint idxCount, out int matIdx);
        return new BakedScene.UnbakedModel(buffer, matIdx, idxCount, idxOffs);
    }

    private static void LoadMeshBuffer(in Mesh mesh, int meshIdx, GraphicsBuffer buffer, out uint indexOffset,
        out uint indexCount, out int materialIndex) {
        AllocateMeshBuffer(mesh, buffer, out indexOffset, out uint vertexOffset, out uint indexLength,
            out uint vertexLength, out indexCount);
        materialIndex = (int)mesh.MMaterialIndex;
        using IMemoryOwner<byte> mem = buffer.MapSlice<byte>(0, buffer.Length);
        Span<byte> span = mem.Memory.Span;
        LoadMeshVerts(mesh,
            MemoryMarshal.Cast<byte, AscensionBasicVertexData>(span.Slice((int)vertexOffset, (int)vertexLength)),
            MemoryMarshal.Cast<byte, uint>(span.Slice((int)indexOffset, (int)indexLength)),
            meshIdx);
    }

    private unsafe void LoadMaterials(in Scene scene, Span<Material> materials) {
        for (int i = 0; i < scene.MNumMaterials; i++) {
            SMaterial* mat = scene.MMaterials[i];
            _assimp.GetMaterialColor(mat, Ass.MaterialColorDiffuseBase, 0, 0, ref materials[i].DiffuseAlbedo)
                .CollectError(_assimp);
            _assimp.GetMaterialFloatArray(mat, Ass.MaterialShininessBase, 0, 0, ref materials[i].Shininess, null)
                .CollectError(_assimp);
            _assimp.GetMaterialColor(mat, Ass.MaterialColorReflectiveBase, 0, 0, ref materials[i].ReflectionFactor)
                .CollectError(_assimp);
        }
    }

    private static unsafe void PreLoadSizes(in Mesh mesh, out uint numVertices, out uint numIndices) {
        numVertices = mesh.MNumVertices;
        numIndices = 0;
        for (int i = 0; i < mesh.MNumFaces; i++) {
            numIndices += mesh.MFaces[i].MNumIndices;
        }
    }

    private static unsafe void LoadMeshVerts(in Mesh mesh, Span<AscensionBasicVertexData> vertices, Span<uint> indices, int meshNum) {
        if (vertices.Length < mesh.MNumVertices)
            throw new ArgumentException($"Insufficient space for vertices for mesh {meshNum}", nameof(vertices));

        for (int i = 0; i < mesh.MNumVertices; i++) {
            vertices[i].Position = mesh.MVertices[i];
            vertices[i].Normal = mesh.MNormals[i];
            vertices[i].Color = new Color(*mesh.MColors[i]);
            vertices[i].TexCoord = ToVector2(*mesh.MTextureCoords[i]);
        }

        uint totalIndices = 0;
        for (int i = 0; i < mesh.MNumFaces; i++) {
            LoadFaceIndices(mesh.MFaces[i], indices[(int)totalIndices..], ref totalIndices, i, meshNum);
        }
    }

    private static unsafe void LoadFaceIndices(in Face face, Span<uint> indices, ref uint numIndices, int faceNum,
        int meshNum) {
        if (indices.Length < (numIndices += face.MNumIndices))
            throw new ArgumentException($"Insufficient space for indices for face {faceNum} of mesh {meshNum}",
                nameof(indices));
        new Span<uint>(face.MIndices, (int)numIndices).CopyTo(indices);
    }

    public void Dispose() {
        _assimp.Dispose();
    }
}