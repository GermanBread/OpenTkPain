#version 330 core
out vec4 FragColor;
in vec4 color;

in vec2 uv;

uniform sampler2D texture1;

void main() {
    vec4 _textureCol = texture(texture1, uv);
    // The way I'd detect no texture is if I offset the UV by +(1,1)
    // No, no need to detect it, just pass A WHITE TEXTURE
    // I am leaving this here as a reminder to myself
    FragColor = color; //* _textureCol;
}