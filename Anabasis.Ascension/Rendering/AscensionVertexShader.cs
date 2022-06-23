using Anabasis.Core.Graphics.Handles;
using Anabasis.Core.Graphics.Shaders;
using Anabasis.Tasks;
using Silk.NET.OpenGL;

namespace Anabasis.Ascension.Rendering;

public sealed class AscensionVertexShader : ITypedShaderStage<AscensionVertexShader>
{
    private const string VertexShaderResourcePath = "Anabasis.Ascension.BasicShaders.ascension_basic.vert";
    public ShaderProgram Program { get; }
    public ProgramHandle Handle => Program.Handle;
    public UseProgramStageMask StageMask => UseProgramStageMask.VertexShaderBit;

    private AscensionVertexShader(ShaderProgram program) {
        Program = program;
    }

    public static async AnabasisTask<AscensionVertexShader> CreateAsync(GL gl) {
        await using Stream? stream =
            typeof(AscensionVertexShader).Assembly.GetManifestResourceStream(VertexShaderResourcePath);
        using StreamReader reader = new(stream ?? throw new FileNotFoundException());
        return new AscensionVertexShader(await ShaderProgram.CreateSeparableShaderProgram(gl,
            ShaderType.VertexShader, await reader.ReadToEndAsync()));
    }

    private uint? _matricesBufferLocation;
    public void BindTransformsUniformBuffer(uint binding) =>
        Program.SetUniformBlockBinding("TransformMatrices", ref _matricesBufferLocation, binding);

    public void Dispose() {
        Program.Dispose();
    }
}