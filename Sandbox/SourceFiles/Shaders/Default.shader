#shader vertex
#version 330 core

layout (location = 0) in vec3 aPos;
layout (location = 1) in vec3 aNormal;
layout (location = 2) in vec2 aTex;

out vec3 normal;
out vec3 currentPos;
out vec2 texCoord;

uniform mat4 model;  // Model matrix
uniform mat4 mvp;    // modelMatrix * viewMatrix * projectionMatrix;

void main()
{
    currentPos = vec3(model * vec4(aPos, 1.0));
    gl_Position = mvp * vec4(aPos, 1.0);
    texCoord = aTex * -1; // Flip then pass through texture coordinates
    normal = aNormal;
}

#shader fragment
#version 330 core

out vec4 FragColor;

in vec2 texCoord;
in vec3 normal;
in vec3 currentPos;

uniform sampler2D tex0;
uniform bool hasTexture;
uniform bool hasDirectionalLight;
uniform vec3 cameraPosition;
uniform mat4 model; // Model matrix

uniform struct Material {
    vec3 albedo;
    float roughness;
    float metalness;
    float ambientOcclusion;
    float specularStrength;
    float opacity;
} material;

uniform struct DirectionalLight {
    vec3 direction;
    vec4 color;
    int followCamera;
} directionalLight;

vec3 calculateDiffuse(vec3 lightDirection, vec3 normalizedNormal) {
    float diffuse = max(dot(normalizedNormal, lightDirection), 0.0f);
    return directionalLight.color.rgb * diffuse * directionalLight.color.a;
}

vec3 calculateSpecular(vec3 viewDirection, vec3 lightDirection, vec3 normalizedNormal) {
    vec3 reflectDirection = reflect(-lightDirection, normalizedNormal);
    float specularAmount = pow(max(dot(viewDirection, reflectDirection), 0.0f), 8);
    float specularFactor = specularAmount * material.specularStrength;
    return directionalLight.color.rgb * specularFactor * directionalLight.color.a;
}

void main()
{
    float ambient = 0.05f;
    vec3 normalizedNormal = normalize(vec3(model * vec4(normal, 1)));
    vec3 viewDirection = normalize(cameraPosition - currentPos);
    
    vec3 lightDirection;
    if (directionalLight.followCamera != 0) {
		lightDirection = normalize(viewDirection);
	}
    else
    {
        lightDirection = normalize(directionalLight.direction);
    }
    
    vec3 diffuseColor = vec3(0.0);
    vec3 specularColor = vec3(0.0);

    if (hasTexture) {
        vec4 texColor = texture(tex0, texCoord);
        vec3 albedo = material.albedo * texColor.rgb;
        FragColor = vec4(albedo, texColor.a);
    } else {
        FragColor = vec4(material.albedo, 1.0);
    }

    diffuseColor = calculateDiffuse(lightDirection, normalizedNormal);
    specularColor = calculateSpecular(viewDirection, lightDirection, normalizedNormal);
    
    if (hasDirectionalLight) FragColor.rgb *= (diffuseColor + specularColor) + ambient;
}