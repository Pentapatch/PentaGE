#shader vertex
#version 330 core

layout (location = 0) in vec3 aPos;
layout (location = 1) in vec3 aNormal;
layout (location = 2) in vec4 aColor;
layout (location = 3) in vec2 aTex;

out vec3 color;
out vec2 texCoord;

uniform mat4 mvp;

void main()
{
    gl_Position = mvp * vec4(aPos, 1.0);
    color = aColor.rgb;   // Use only the RGB components of the color attribute
    texCoord = aTex * -1; // Flip then pass through texture coordinates
}

#shader fragment
#version 330 core

out vec4 FragColor;

in vec3 color;
in vec2 texCoord;

uniform sampler2D tex0;

void main()
{
	FragColor = texture(tex0, texCoord);
}