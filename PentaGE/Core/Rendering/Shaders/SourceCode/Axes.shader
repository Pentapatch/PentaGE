#shader vertex
#version 330 core

// This shader is used to render a gizmo in the bottom-left corner of the screen
// It is used to show the orientation of the camera in the scene (like Unreal Engine's gizmo)

layout (location = 0) in vec3 aPos;
layout (location = 1) in vec3 aNormal;

uniform mat4 view;
uniform vec2 viewportSize;

out vec3 color; 

void main()
{
    int x = 50; // Gizmo placement in pixels from the left of the screen
    int y = 50; // Gizmo placement in pixels from the bottom of the screen
    
    // Calculate the screen position of the vertex
    vec2 screenPos = vec2(1 / (viewportSize.x / 2) * x - 1, 1 / (viewportSize.y / 2) * y - 1);
    
    // Rotate the gizmo according to the camera rotation
    mat3 cameraRotation = mat3(view);
    vec3 rotatedPosition = cameraRotation * aPos;
    
    // Scale the rotated vertex positions to account for aspect ratio
    float aspectRatio = viewportSize.y / viewportSize.x;
    rotatedPosition.x *= aspectRatio;

    // Transform the rotated vertex to the screen-space position
    gl_Position = vec4(vec3(screenPos.xy, 0) + rotatedPosition, 1.0);

    // Set the color of the gizmo
    if(aNormal.x > 0.0)
        color = vec3(1.0, 0.0, 0.0);
    else if (aNormal.y > 0)
        color = vec3(0.0, 1.0, 0.0);
    else if (aNormal.z > 0)
        color = vec3(0.0, 0.0, 1.0);
}

#shader fragment
#version 330 core

in vec3 color;

layout(location = 0) out vec4 fragColor;

void main()
{
    fragColor = vec4(color.rgb, 1.0);
}