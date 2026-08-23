#version 330 core
                                    
in vec2 frag_texCoords;
out vec4 out_color;

uniform sampler2D uTexture;
uniform vec3 uBaseColor = vec3(1,1,1);


void main()
{
    out_color = texture(uTexture, frag_texCoords) * vec4(uBaseColor.xyz, 1);
}