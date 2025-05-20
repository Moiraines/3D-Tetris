#version 330 core

// === Output color ===
out vec4 FragColor;

// === Inputs from Vertex Shader ===
in vec2 TexCoord;        // Interpolated texture coordinates
in float DepthFactor;    // Normalized depth value (0 = far, 1 = near)

// === Uniforms ===
uniform sampler2D tetrominoTexture; // Texture for the cube face

void main()
{
    // Sample the texture color using interpolated texture coordinates
    vec4 texColor = texture(tetrominoTexture, TexCoord);

    // Compute adjusted depth factor using smoothstep for a soft falloff
    float adjustedDepth = smoothstep(0.5, 0.1, DepthFactor);

    // Darken the color based on depth (farther cubes look darker)
    texColor.rgb *= mix(1.0, 0.4, adjustedDepth);

    // Output final fragment color
    FragColor = texColor;
}