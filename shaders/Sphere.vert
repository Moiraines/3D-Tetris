#version 330 core

// === Input vertex attributes ===
// aPos: Position of the vertex in object space
// aNormal: Normal vector at the vertex, used for lighting
layout (location = 0) in vec3 aPos;
layout (location = 1) in vec3 aNormal;

// === Transformation matrices ===
uniform mat4 model;       // Transforms object to world space
uniform mat4 view;        // Transforms world space to camera space
uniform mat4 projection;  // Transforms camera space to clip space

// === Outputs to Fragment Shader ===
out vec3 FragPos;  // World space position of the vertex
out vec3 Normal;   // World space normal, adjusted for model transformation

void main()
{
    // Convert vertex position from object space to world space
    FragPos = vec3(model * vec4(aPos, 1.0));

    // Correct normal transformation (handles scaling/skew)
    Normal = mat3(transpose(inverse(model))) * aNormal;

    // Final position of vertex in clip space
    gl_Position = projection * view * vec4(FragPos, 1.0);
}