#version 330 core

// === Output color of the pixel ===
out vec4 FragColor;

// === Texture coordinates passed from vertex shader ===
in vec2 TexCoord;

// === Texture sampler (2D texture bound to the floor) ===
uniform sampler2D floorTexture;

void main()
{
    // Sample the floor texture using the given texture coordinates
    FragColor = texture(floorTexture, TexCoord);
}