#version 330 core
layout (location = 0) in vec2 aPosition;
layout (location = 3) in vec4 aColor;

out vec4 color;

void main()
{
    color = aColor;
    
    gl_Position = vec4(aPosition, 0.0, 1.0);
}