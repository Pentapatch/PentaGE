#shader vertex
#version 330 core

layout (location = 0) in vec2 aPos;
layout (location = 1) in vec3 aColor;

uniform mat4 mvp;

out vec3 color;

void main()
{
    gl_Position = mvp * vec4(aPos, 0.0, 1.0);
    color = aColor;
}

#shader fragment
#version 330 core

layout(location = 0) out vec4 color;

void main()
{
	color = vec4(1.0, 0.0, 0.0, 1.0);
};