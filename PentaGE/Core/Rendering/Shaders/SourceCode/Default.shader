#shader vertex
#version 330 core

layout (location = 0) in vec3 aPos;
layout (location = 1) in vec3 aNormal;
layout (location = 2) in vec4 aColor;
layout (location = 3) in vec2 aTex;

out vec3 color;
out vec3 normal;
out vec3 currentPos;
out vec2 texCoord;

uniform mat4 model;  // Model matrix
uniform mat4 mvp;    // modelMatrix * viewMatrix * projectionMatrix;

void main()
{
    currentPos = vec3(model * vec4(aPos, 1.0));
    gl_Position = mvp * vec4(aPos, 1.0);
    color = aColor.rgb;
    texCoord = aTex * -1; // Flip then pass through texture coordinates
    normal = aNormal;
}

#shader fragment
#version 330 core

out vec4 FragColor;

in vec3 color;
in vec2 texCoord;
in vec3 normal;
in vec3 currentPos;

uniform sampler2D tex0;
uniform vec4 lightColor;
uniform vec3 lightPosition;
uniform vec3 cameraPosition;
uniform mat4 model; // Model matrix

void main()
{
    float ambient = 0.05f;
    vec3 normalizedNormal = normalize(vec3(model * vec4(normal, 1)));
    
    vec3 lightDirection = normalize(lightPosition - currentPos);
    float diffuse = max(dot(normalizedNormal, lightDirection), 0.0f);

    float specularStrength = 0.5f;
    vec3 viewDirection = normalize(cameraPosition - currentPos);
    vec3 reflectDirection = reflect(-lightDirection, normalizedNormal);
    float specularAmount = pow(max(dot(viewDirection, reflectDirection), 0.0f), 8);
    float specular = specularAmount * specularStrength;

	FragColor = texture(tex0, texCoord) * lightColor * (diffuse + ambient);
}