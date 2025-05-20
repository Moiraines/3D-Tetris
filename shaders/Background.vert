#version 330 core

// === Vertex Attributes ===
// aPos: Position of the vertex in object (local) space
// aTexCoord: Texture coordinates (u, v)
layout(location = 0) in vec3 aPos;
layout(location = 1) in vec2 aTexCoord;

// === Passed to Fragment Shader ===
out vec2 TexCoord;

// === Uniform Matrices for Transformations ===
// model: local -> world
// view: world -> camera
// projection: camera -> screen
uniform mat4 model;
uniform mat4 view;
uniform mat4 projection;

void main()
{
    // Compute final vertex position in clip space
    gl_Position = projection * view * model * vec4(aPos, 1.0);

    // Pass texture coordinate to fragment shader
    TexCoord = aTexCoord;
}