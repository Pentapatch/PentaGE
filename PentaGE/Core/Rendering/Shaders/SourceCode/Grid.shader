#shader vertex
#version 330 core

layout (location = 0) in vec3 aPos;
layout (location = 1) in vec3 aNormal;

out vec3 normal;

uniform mat4 mvp;

void main()
{
    gl_Position = mvp * vec4(aPos, 1.0);
    normal = aNormal;
}

#shader fragment
#version 330 core

out vec4 FragColor;

in vec3 normal;

uniform struct Material {
    vec3 albedo;
    float roughness;
    float metalness;
    float ambientOcclusion;
    float specularStrength;
    float opacity;
} material;

void main()
{
    FragColor = vec4(material.albedo, material.opacity);
}