using Anabasis.Core.Shaders;

namespace Anabasis.Ascension.Rendering;

public static class AscensionShaders
{
    public static void AttachProgram(this ShaderPipeline pipeline, ITypedShaderStage stage) =>
        pipeline.AttachProgram(stage.StageMask, stage.Program);
}