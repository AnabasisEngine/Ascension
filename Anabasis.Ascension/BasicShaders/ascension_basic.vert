#version 330 core
layout (location = 0) in vec3 vPos;
layout (location = 1) in vec4 vColor;
layout (location = 2) in vec2 vUv;
layout (location = 3) in vec3 vNormal;

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
    vec4 fColor;
    vec3 fNormal;
} v_out;

void main()
{
    gl_Position = Projection * View * Model * vec4(vPos, 1.0);
    v_out.fUv = vUv;
    v_out.fColor = vColor;
    v_out.fNormal = mat3(transpose(inverse(Model))) * vNormal;
}