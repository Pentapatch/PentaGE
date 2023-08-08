#shader vertex
#version 330 core

layout (location = 0) in vec3 aPos;
layout (location = 1) in vec3 aNormal;

uniform mat4 mvp;
uniform mat4 model;

out vec3 FragPos;
out vec3 Normal;

void main()
{
    FragPos = vec3(model * vec4(aPos, 1.0));
    Normal = aNormal;
    gl_Position = mvp * vec4(aPos, 1.0);
}

#shader geometry
#version 330 core
layout(triangles) in;
layout(line_strip, max_vertices = 2) out;

in vec3 FragPos[];
in vec3 Normal[];

out vec3 color;

uniform mat4 projection;
uniform mat4 view;

void main()
{
    float scale = 0.2f; //                       Adjust the length of the normal line
    vec3 startColor = vec3(0.0f, 0.0f, 0.0f); // Adjust the color at the origin
    vec3 endColor = vec3(1.0f, 0.0f, 0.0f); //   Adjust the color at the end
    vec3 normal = normalize(Normal[0]);
    vec3 midpoint = (FragPos[0] + FragPos[1] + FragPos[2]) / 3.0;
    vec3 endpoint = midpoint + normal * scale;

    gl_Position = projection * view * vec4(midpoint, 1.0);
    color = startColor;
    EmitVertex();

    gl_Position = projection * view * vec4(endpoint, 1.0);
    color = endColor;
    EmitVertex();

    EndPrimitive();
}

#shader fragment
#version 330 core

in vec3 color;

layout(location = 0) out vec4 fragColor;

void main()
{
    fragColor = vec4(color.rgb, 1.0);
}