#version 450
const float PI = 3.14159265359;

in vec2 outTexCoords;
in vec4 fragPosLightSpace;
in vec3 outNormal;
in vec3 worldPos;

out vec4 fragColor;

struct Material {
    sampler2D texture_diffuse1;
    sampler2D texture_diffuse2;

    sampler2D texture_roughness1;

    sampler2D texture_normal1;
};

uniform Material material;
uniform sampler2D shadowMap;
uniform vec3 camPos;
uniform vec3 lightPos;
uniform float roughness;

uniform float metallic;
uniform float ao;

// Shadow mapping
float CalcShadows(vec4 fragPos)
{
    float bias = 0.1;

    vec3 projectedCoords = fragPos.xyz / fragPos.w;
    projectedCoords = projectedCoords * 0.5 + 0.5;

    float currentDepth = projectedCoords.z;

    float shadow = 0.0;
    vec2 texelSize = 1.0 / textureSize(shadowMap, 0);
    for (int x = -2; x <= 2; ++x)
    { 
        for (int y = -2; y <= 2; ++y)
        {
            float pcfDepth = texture(shadowMap, projectedCoords.xy + vec2(x, y) * texelSize).r;
            shadow += currentDepth - bias > pcfDepth ? 1.0 : 0.0;
        }
    }
    shadow /= 25.0;

    if (projectedCoords.z > 1.0)
        shadow = 0.0;

    return shadow;
}

// PBR calculations
float DistributionGGX(vec3 N, vec3 H, float roughness)
{
    float a = roughness * roughness;
    float a2 = a * a;
    float NdotH = max(dot(N, H), 0.0);
    float NdotH2 = NdotH * NdotH;

    float denom = (NdotH2 * (a2 - 1.0) + 1.0);
    denom = PI * denom * denom;

    return a2 / denom;
}

float GeometrySchlickGGX(float NdotV, float roughness)
{
    float r = (roughness + 1.0);
    float k = (r * r) / 8.0;

    float denom = NdotV * (1.0 - k) + k;
    return NdotV / denom;
}

float GeometrySmith(vec3 N, vec3 V, vec3 L, float roughness)
{
    float NdotV = max(dot(N, V), 0.0);
    float NdotL = max(dot(N, L), 0.0);
    float ggx1 = GeometrySchlickGGX(NdotL, roughness);
    float ggx2 = GeometrySchlickGGX(NdotV, roughness);

    return ggx1 * ggx2;
}

vec3 FresnelSchlick(float cosTheta, vec3 F0)
{
    return F0 + (1.0 - F0) * pow(1.0 - cosTheta, 5.0);
}

void main() {
    vec3 albedo = texture(material.texture_diffuse1, outTexCoords).xyz;
    // float roughness = texture(material.texture_roughness1, outTexCoords).x;

    // vec3 normal = texture(material.texture_normal1, outTexCoords).xyz;
    // normal = normalize(normal * 2.0 - 1.0);

    vec3 N = normalize(outNormal);
    vec3 V = normalize(camPos - worldPos);
    
    vec3 F0 = vec3(0.04); 
    F0 = mix(F0, albedo, metallic);

    vec3 Lo = vec3(0.0);
    vec3 L = normalize(lightPos - worldPos);
    vec3 H = normalize(V + L);
    float distance = length(lightPos - worldPos);
    float attenuation = 10.0 / (distance * distance); // *10
    vec3 radiance = vec3(1.0, 1.0, 1.0) * attenuation;

    float NDF = DistributionGGX(N, H, roughness);
    float G = GeometrySmith(N, V, L, roughness);
    vec3 F = FresnelSchlick(max(dot(H, V), 0.0), F0);

    vec3 kS = F;
    vec3 kD = vec3(1.0) - kS;
    kD *= 1.0 - metallic;

    vec3 numerator = NDF * G * F;
    float denominator = 4.0 * max(dot(N, V), 0.0) * max(dot(N, L), 0.0);
    vec3 specular = numerator / max(denominator, 0.001);

    float NdotL = max(dot(N, L), 0.0);
    Lo += (kD * albedo / PI + specular) * radiance * NdotL;

    vec3 ambient = vec3(0.03) * albedo * ao;
    vec3 color = ambient + Lo;

    color = color / (color + vec3(1.0));
    color = pow(color, vec3(1.0 / 2.2));

    fragColor = vec4(color - (CalcShadows(fragPosLightSpace) * 0.25), 1.0);
}