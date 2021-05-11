#version 330 core
out vec4 FragColor;
in vec4 color;

in vec2 uv;
in vec2 innerPos;

uniform sampler2D texture1;

void main() {
    vec4 _textureCol = texture(texture1, uv);

    FragColor = color * _textureCol;
    /*if (innerPos.x < .1f)
        FragColor = vec4(1, 0, 0, 1);
    if (innerPos.y < .1f)
        FragColor = vec4(0, 1, 0, 1);
    if (innerPos.y < .1f && innerPos.x < .1f)
        FragColor = vec4(1, 1, 0, 1);*/
}