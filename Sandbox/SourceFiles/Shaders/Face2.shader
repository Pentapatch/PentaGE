#shader vertex
#version 330 core

// This shader is calculating the normal vector of the triangle
// (instead of taking the provided normal) and then using it to color the triangle

layout (location = 0) in vec3 aPos;

uniform mat4 mvp;

out vec3 FragPos;

void main()
{
    FragPos = vec3(aPos.xyz);
    gl_Position = mvp * vec4(aPos, 1.0);
}

#shader geometry
#version 330 core

layout (triangles) in;
layout (triangle_strip, max_vertices = 3) out;

in vec3 FragPos[];

out vec3 fragColor;
out vec3 fragNormal;

void main()
{
    vec3 AB = FragPos[1] - FragPos[0];
    vec3 AC = FragPos[2] - FragPos[0];
    fragNormal = normalize(cross(AB, AC));

    for (int i = 0; i < gl_in.length(); ++i)
    {
        fragColor = vec3((fragNormal.x + 1.0) * 0.25 + 0.5,
                        (fragNormal.y + 1.0) * 0.25 + 0.5,
                        (fragNormal.z + 1.0) * 0.25 + 0.5);
        gl_Position = gl_in[i].gl_Position;
        EmitVertex();
    }

    EndPrimitive();
}

#shader fragment
#version 330 core

in vec3 fragColor;

layout(location = 0) out vec4 fragOutput;

void main()
{
    fragOutput = vec4(fragColor, 1.0);
}