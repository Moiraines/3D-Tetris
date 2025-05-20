#version 330 core

// === Inputs from Vertex Shader ===
in vec3 FragPos;  // Fragment position in world space
in vec3 Normal;   // Normal vector in world space

// === Output color ===
out vec4 FragColor;

// === Uniforms ===
uniform vec3 color;                     // Base color of the shape (e.g., sphere)
uniform vec3 lightPos = vec3(20.0);     // Position of the light source
uniform vec3 lightColor = vec3(1.0);    // Color of the light
uniform vec3 viewPos = vec3(0.0, 0.0, 20.0); // Position of the camera/viewer

void main()
{
    // === Ambient Lighting ===
    float ambientStrength = 0.5;
    vec3 ambient = ambientStrength * lightColor;

    // === Diffuse Lighting (Lambert) ===
    vec3 norm = normalize(Normal);
    vec3 lightDir = normalize(lightPos - FragPos);
    float diff = max(dot(norm, lightDir), 0.0);
    vec3 diffuse = diff * lightColor;

    // === Specular Lighting (Blinn-Phong) ===
    float specularStrength = 0.6;
    vec3 viewDir = normalize(viewPos - FragPos);
    vec3 reflectDir = reflect(-lightDir, norm);
    float spec = pow(max(dot(viewDir, reflectDir), 0.0), 16.0); // Shininess factor
    vec3 specular = specularStrength * spec * lightColor;

    // === Final Color Calculation ===
    vec3 result = (ambient + diffuse + specular) * color;
    FragColor = vec4(result, 1.0);
}