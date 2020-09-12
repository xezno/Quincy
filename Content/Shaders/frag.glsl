#version 450

in vec2 outTexCoords;
in vec4 fragPosLightSpace;

out vec4 fragColor;

struct Material {
    sampler2D texture_diffuse1;
    sampler2D texture_diffuse2;
};

uniform Material material;
uniform sampler2D shadowMap;

float CalcShadows(vec4 fragPos)
{
    float bias = 0.1;

    vec3 projectedCoords = fragPos.xyz / fragPos.w;
    projectedCoords = projectedCoords * 0.5 + 0.5;

    float closestDepth = texture(shadowMap, projectedCoords.xy).r;
    float currentDepth = projectedCoords.z;

    float shadow = 0.0;
    vec2 texelSize = 1.0 / textureSize(shadowMap, 0);
    for (int x = -1; x <= 1; ++x)
    { 
        for (int y = -1; y <= 1; ++y)
        {
            float pcfDepth = texture(shadowMap, projectedCoords.xy + vec2(x, y) * texelSize).r;
            shadow += currentDepth - bias > pcfDepth ? 1.0 : 0.0;
        }   
    }
    shadow /= 9.0;

    return shadow;
}

void main() {
    vec4 textureValue = mix(texture(material.texture_diffuse1, outTexCoords), texture(material.texture_diffuse2, outTexCoords), 0.5);
    fragColor = textureValue - (CalcShadows(fragPosLightSpace));
}