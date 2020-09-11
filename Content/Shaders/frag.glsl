#version 450

in vec2 outTexCoords;
out vec4 fragColor;

struct Material {
    sampler2D texture_diffuse1;
    sampler2D texture_diffuse2;
};

uniform Material material;

void main() {
    fragColor = mix(texture(material.texture_diffuse1, outTexCoords), texture(material.texture_diffuse2, outTexCoords), 0.5);
}