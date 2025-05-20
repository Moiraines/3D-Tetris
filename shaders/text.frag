#version 330 core

// Final color output
out vec4 FragColor;

// Input from vertex shader
in vec2 TexCoord;

// Monochrome font texture (grayscale with alpha)
uniform sampler2D Texttexture;

// The desired color for the text (RGB)
uniform vec3 color;

void main()
{
    // Sample alpha value from the texture (grayscale sprite)
    float alpha = texture(Texttexture, TexCoord).a;

    // Multiply input color with sampled alpha to preserve transparency
    FragColor = vec4(color, 1.0) * alpha;
}