#version 330 core
out vec4 FragColor;
in vec4 color;
in vec2 uv;

uniform sampler2D texture0;

void main() {
    float _mixRatio = .5;
    vec4 _textureCol = texture(texture0, uv);

    // Detecting if there's a texture present
    if (_textureCol.x == 0)
        _mixRatio = 1;
    FragColor = mix(_textureCol, color, _mixRatio);
}