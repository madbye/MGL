#define LOWP

uniform sampler2D TextureSampler;

varying vec4 v_color;
varying vec2 v_texCoords;

void main()
{
    vec4 texColor = texture2D(TextureSampler, v_texCoords);
    gl_FragColor = texColor * v_color;
}