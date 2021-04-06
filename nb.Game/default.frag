#version 330 core
out vec4 FragColor;
in vec4 color;
in vec2 uv;

uniform sampler2D texture1;

void main() {
    vec4 _textureCol = texture(texture1, uv);
    if (_textureCol == vec4(0))
        _textureCol = vec4(1);
    FragColor = _textureCol * color * FragColor;
}