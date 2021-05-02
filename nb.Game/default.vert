#version 330 core
layout (location = 0) in vec2 aPosition;
layout (location = 1) in vec2 UV;
layout (location = 2) in vec2 aInnerPos;
layout (location = 3) in vec4 aColor;

out vec4 color;
out vec2 uv;
out vec2 innerPos;

void main()
{
    uv = UV;
    innerPos = aInnerPos;
    
    color = aColor;

    gl_Position = vec4(aPosition, 0.0, 1.0);
}