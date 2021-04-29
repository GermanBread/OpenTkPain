#version 330 core
layout (location = 0) in vec2 aPosition;
layout (location = 1) in vec2 UV;
layout (location = 2) in vec4 aColor;

out vec4 color;
out vec2 uv;

void main()
{
    uv = UV;
    
    color = aColor;

    gl_Position = vec4(aPosition, 0.0, 1.0);
}