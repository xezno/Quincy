#version 450

layout(location = 0) in vec3 position;
layout(location = 1) in vec3 normal;
layout(location = 2) in vec2 texCoords;

out vec2 outTexCoords;

uniform mat4 modelMatrix;
uniform mat4 viewMatrix;
uniform mat4 projectionMatrix;

void main() {
    outTexCoords = texCoords;
    gl_Position = projectionMatrix * viewMatrix * modelMatrix * vec4(position, 1.0);
}