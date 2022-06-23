using Anabasis.Ascension.Rendering;
using Anabasis.Core.Graphics.Handles;
using Anabasis.Core.Graphics.Shaders;
using Anabasis.Tasks;
using Silk.NET.OpenGL;

namespace Anabasis.Ascension.Model;

public class Basic3dFragmentShader
{
    private const string VertexShaderResourcePath = "Anabasis.Ascension.BasicShaders.basic_3d.vert";
    public ShaderProgram Program { get; }
    public ProgramHandle Handle => Program.Handle;
    public UseProgramStageMask StageMask => UseProgramStageMask.FragmentShaderBit;

    private Basic3dFragmentShader(ShaderProgram program) {
        Program = program;
    }

    public static async AnabasisTask<Basic3dFragmentShader> CreateAsync(GL gl) {
        await using Stream? stream =
            typeof(AscensionVertexShader).Assembly.GetManifestResourceStream(VertexShaderResourcePath);
        using StreamReader reader = new(stream ?? throw new FileNotFoundException());
        return new Basic3dFragmentShader(await ShaderProgram.CreateSeparableShaderProgram(gl,
            ShaderType.FragmentShader, await reader.ReadToEndAsync()));
    }

    private uint? _matricesBufferLocation;
    public void BindMaterialBuffer(uint binding) =>
        Program.SetUniformBlockBinding("Material", ref _matricesBufferLocation, binding);

    public void Dispose() {
        Program.Dispose();
    }
}