using Anabasis.Core.Handles;
using Anabasis.Core.Shaders;
using Anabasis.Tasks;
using Silk.NET.OpenGL;

namespace Anabasis.Ascension.Rendering;

public interface ITypedShaderStage : IDisposable
{
    public ShaderProgram Program { get; }
    public ProgramHandle Handle => Program.Handle;
    
    public UseProgramStageMask StageMask { get; }
}

public interface ITypedShaderStage<T> : ITypedShaderStage
    where T : ITypedShaderStage<T>
{
    public static abstract AnabasisTask<T> CreateAsync(GL gl);
}