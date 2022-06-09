#version 330 core
layout (location = 0) in vec3 vPos;
layout (location = 1) in vec2 vUv;

layout (std140, row_major) uniform TransformMatrices {
    mat4 Model;
    mat4 View;
    mat4 Projection;
};

out gl_PerVertex
{
    vec4 gl_Position;
};

out v_out
{
    vec2 fUv;
} v_out;

void main()
{
    gl_Position = TransformMatrices.Projection * TransformMatrices.View * TransformMatrices.Model * vec4(vPos, 1.0);
    v_out.fUv = vUv;
}