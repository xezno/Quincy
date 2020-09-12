#version 450

layout(location = 0) in vec3 position;
layout(location = 1) in vec3 normal;
layout(location = 2) in vec2 texCoords;

out vec2 outTexCoords;
out vec3 outNormal;
out vec3 worldPos; // TODO
out vec4 fragPosLightSpace;

uniform mat4 modelMatrix;
uniform mat4 viewMatrix;
uniform mat4 projectionMatrix;

uniform mat4 lightViewMatrix;
uniform mat4 lightProjectionMatrix;

void main() {
    worldPos = vec3(modelMatrix * vec4(position, 1.0));
    outTexCoords = texCoords;
    outNormal = normal;
    fragPosLightSpace = lightProjectionMatrix * lightViewMatrix * vec4(worldPos, 1.0);
    
    gl_Position = projectionMatrix * viewMatrix * vec4(worldPos, 1.0);
}