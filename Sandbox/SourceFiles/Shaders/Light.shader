#shader vertex
#version 330 core

layout (location = 0) in vec3 aPos;
layout (location = 1) in vec3 aNormal;
layout (location = 2) in vec2 aTex;

uniform mat4 mvp;
uniform vec4 lightColor;

void main()
{
    gl_Position = mvp * vec4(aPos, 1.0);
}

#shader fragment
#version 330 core

out vec4 FragColor;

uniform vec4 lightColor;

void main()
{
	FragColor = lightColor;
}