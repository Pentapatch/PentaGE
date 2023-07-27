#shader vertex
#version 330 core

layout (location = 0) in vec3 aPos;
layout (location = 1) in vec3 aColor;

uniform mat4 mvp;

out vec3 color;

void main()
{
    gl_Position = mvp * vec4(aPos, 1.0);
    color = aColor;
}

#shader fragment
#version 330 core

in vec3 color;

layout(location = 0) out vec4 fragColor;

void main()
{
    fragColor = vec4(color, 1.0);
}