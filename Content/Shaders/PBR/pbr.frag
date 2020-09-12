#version 450
const float PI = 3.14159265359;

in VS_OUT {
    vec2 texCoords;
    vec3 normal;
    vec3 worldPos;
    vec4 fragPosLightSpace;

    vec3 tangentLightPos;
    vec3 tangentCamPos;
    vec3 tangentWorldPos;
} vs_out;

out vec4 fragColor;

struct Material {
    sampler2D texture_diffuse1;
    sampler2D texture_diffuse2;

    sampler2D texture_emissive1;

    sampler2D texture_unknown1;

    sampler2D texture_normal1;
};

uniform Material material;
uniform sampler2D shadowMap;

// Shadow mapping
float CalcShadows(vec4 fragPos)
{
    float bias = 0.1;

    vec3 projectedCoords = fragPos.xyz / fragPos.w;
    projectedCoords = projectedCoords * 0.5 + 0.5;

    float currentDepth = projectedCoords.z;

    float shadow = 0.0;
    vec2 texelSize = 1.0 / textureSize(shadowMap, 0);
    for (int x = -2; x <= 1; ++x)
    { 
        for (int y = -2; y <= 1; ++y)
        {
            float pcfDepth = texture(shadowMap, projectedCoords.xy + vec2(x, y) * texelSize).r;
            shadow += currentDepth - bias > pcfDepth ? 1.0 : 0.0;
        }
    }
    shadow /= 16.0;

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
    vec4 albedoSrc = texture(material.texture_diffuse1, vs_out.texCoords);
    vec3 albedo = albedoSrc.xyz;
    float ao = texture(material.texture_unknown1, vs_out.texCoords).x;
    float roughness = texture(material.texture_unknown1, vs_out.texCoords).y;
    float metallic = texture(material.texture_unknown1, vs_out.texCoords).z;

    vec3 normal = texture(material.texture_normal1, vs_out.texCoords).xyz;
    normal = normalize(normal * 2.0 - 1.0);

    vec3 N = normalize(normal);
    vec3 V = normalize(vs_out.tangentCamPos - vs_out.tangentWorldPos);
    
    vec3 F0 = vec3(0.04); 
    F0 = mix(F0, albedo, metallic);

    vec3 Lo = vec3(0.0);
    vec3 L = normalize(vs_out.tangentLightPos - vs_out.tangentWorldPos);
    vec3 H = normalize(V + L);
    float distance = length(vs_out.tangentLightPos - vs_out.tangentWorldPos);
    float attenuation = 1.0 / (distance * distance);
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
    // color = pow(color, vec3(1.0 / 2.2));
    color = pow(color, vec3(1.0 / 3.1));

    vec4 emissive = texture(material.texture_emissive1, vs_out.texCoords);
    // color += emissive.xyz;

    fragColor = vec4(color - (CalcShadows(vs_out.fragPosLightSpace) * 0.03), albedoSrc.w);

    // fragColor = vec4(vec3(1.0 - CalcShadows(vs_out.fragPosLightSpace)), 1.0);
}