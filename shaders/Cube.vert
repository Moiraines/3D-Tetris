#version 330 core

// === Vertex Attributes ===
// aPos: Vertex position in object space
// aTexCoord: Texture coordinates
layout(location = 0) in vec3 aPos;
layout(location = 1) in vec2 aTexCoord;

// === Outputs to Fragment Shader ===
out vec2 TexCoord;       // Passes texture coordinates to the fragment shader
out float DepthFactor;   // Used to influence color/appearance based on depth

// === Uniforms (External Inputs) ===
uniform mat4 model;      // Model matrix for object transformation
uniform mat4 view;       // View matrix for camera
uniform mat4 projection; // Projection matrix for perspective view

uniform float minDepth;  // Minimum depth value (unused here, but declared)
uniform float maxDepth;  // Maximum depth value (unused here, but declared)

void main()
{
    // Calculate final position of vertex in clip space
    gl_Position = projection * view * model * vec4(aPos, 1.0);

    // Pass texture coordinates to fragment shader
    TexCoord = aTexCoord;

    // Compute view-space depth of the vertex
    float rawDepth = abs((view * model * vec4(aPos, 1.0)).z);

    // Normalize depth to [0,1] range, invert so closer = higher
    DepthFactor = 1.0 - clamp(rawDepth / 50.0, 0.0, 1.0);
}