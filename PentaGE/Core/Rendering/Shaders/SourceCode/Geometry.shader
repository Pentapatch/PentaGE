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

#shader geometry
#version 330 core
// Just passing through for testing purposes
layout (lines) in;
layout (line_strip, max_vertices = 2) out;

in vec3 normal[];
out vec3 Normal;

uniform mat4 mvp;

void main()
{
    for (int i = 0; i < gl_in.length(); i++) {
        gl_Position = gl_in[i].gl_Position;
        Normal = normal[i];
        EmitVertex();
    }
    EndPrimitive();
}

#shader fragment
#version 330 core

out vec4 FragColor;

in vec3 Normal;

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
    FragColor = vec4(Normal, material.opacity);
}