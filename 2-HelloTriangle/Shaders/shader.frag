#version 430

in vec3 Color;
out vec4 outputColor;

void main()
{
    outputColor = vec4(Color,1.0);
}