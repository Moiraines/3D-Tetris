#version 330 core

// === Vertex attributes ===
layout(location = 0) in vec3 position;         // Vertex position in object space
layout(location = 1) in vec2 textureCoords;    // Corresponding texture coordinates

// === Output to fragment shader ===
out vec2 TexCoord;

// === Uniforms from CPU-side ===
uniform mat4 projection;   // Orthographic projection matrix (screen space)
uniform mat4 model;        // Model transformation matrix (position, scale, etc.)

void main()
{
    // Compute final screen position of the vertex
    gl_Position = projection * model * vec4(position, 1.0f);

    // Pass texture coordinates to fragment shader
    TexCoord = vec2(textureCoords.x, textureCoords.y);
}