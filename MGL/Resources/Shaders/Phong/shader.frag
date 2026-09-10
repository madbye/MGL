#version 330 core
                                    
in vec2 frag_texCoords;
out vec4 out_color;

uniform sampler2D uTexture;

uniform vec3 lightPos;
uniform vec3 lightColor = vec3(1, 1, 1);

uniform vec3 viewPos;

uniform float specularStrength = 0.4f;
uniform float ambientStrength = 0.6f;

in vec3 FragPos;  
in vec3 Normal;

struct Light {
    int type;

    vec3 position;
    vec3 direction;
    vec3 color;

    float constant;
    float linear;
    float quadratic;

    float cutOff;
    float outerCutOff;

    float radius;
};

#define NR_LIGHTS 16
uniform Light lights[NR_LIGHTS];
uniform int numLights;

void main()
{
    vec3 norm = normalize(Normal);
    vec3 lightDir = normalize(lightPos - FragPos);

    float diff = max(dot(norm, lightDir), 0.0);
    vec3 diffuse = diff * lightColor;

    vec3 ambient = ambientStrength * lightColor;

    vec3 viewDir = normalize(viewPos - FragPos);
    vec3 reflectDir = reflect(-lightDir, norm);


    float spec = pow(max(dot(viewDir, reflectDir), 0.0), 16);
    vec3 specular = specularStrength * spec * vec3(1,1,1);

    vec3 result = ambient + diffuse + specular;
    
    out_color =  vec4(result, 1.0) * texture(uTexture, frag_texCoords);
}