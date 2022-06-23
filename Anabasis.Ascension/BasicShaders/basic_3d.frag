#version 330 core

in v_out
{
    vec2 fUv;
    vec4 fColor;
    vec3 fNormal;
} v_out;

layout (std140, row_major) Material {
    vec3 objectColor;
    vec3 lightColor;
    vec3 lightPos;
};

out vec4 FragColor;

void main()
{
    float ambientStrength = 0.1;
    vec3 ambient = ambientStrength * lightColor;

    vec3 norm = normalize(fNormal);
    vec3 lightDirection = normalize(lightPos - fPos);
    float diff = max(dot(norm, lightDirection), 0.0);
    vec3 diffuse = diff * lightColor;

    //The resulting colour should be the amount of ambient colour + the amount of additional colour provided by the diffuse of the lamp
    vec3 result = (ambient + diffuse) * objectColor;

    FragColor = vec4(result, 1.0);
}