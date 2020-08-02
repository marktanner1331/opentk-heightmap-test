#version 330 core

layout(location = 0) in vec3 aPosition;

out float height;

uniform mat4 model;
uniform mat4 view;
uniform mat4 projection;

void main(void)
{
    height = aPosition.y;
    gl_Position = vec4(aPosition, 1.0) * model * view * projection;
}