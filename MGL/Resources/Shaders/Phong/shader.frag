#version 330 core

in vec2 frag_texCoords;
in vec3 FragPos;
in vec3 Normal;

out vec4 out_color;

uniform sampler2D uTexture;
uniform vec3 viewPos;

uniform float specularStrength = 0.4f;
uniform float ambientStrength = 0.6f;

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
    vec3 viewDir = normalize(viewPos - FragPos);

    vec3 totalDiffuse = vec3(0.0);
    vec3 totalSpecular = vec3(0.0);

    for(int i = 0; i < numLights; i++) {
        vec3 lightDir = vec3(0.0);
        vec3 lightColor = lights[i].color;
        float attenuation = 1.0;

        if (lights[i].type == 0) {
            lightDir = normalize(-lights[i].direction);
        }
        else {
            float distance = length(lights[i].position - FragPos);

            if (distance > lights[i].radius) {
                continue;
            }

            lightDir = normalize(lights[i].position - FragPos);
            attenuation = 1.0 / (lights[i].constant + lights[i].linear * distance + lights[i].quadratic * (distance * distance));

            if (lights[i].type == 2) {
                float theta = dot(lightDir, normalize(-lights[i].direction));
                float epsilon = lights[i].cutOff - lights[i].outerCutOff;
                float intensity = clamp((theta - lights[i].outerCutOff) / max(epsilon, 0.0001), 0.0, 1.0);
                attenuation *= intensity;
            }
        }
        
        float diff = max(dot(norm, lightDir), 0.0);
        vec3 diffuse = diff * lightColor * attenuation;
        
        vec3 halfwayDir = normalize(lightDir + viewDir);
        float spec = pow(max(dot(norm, halfwayDir), 0.0), 32.0);
        vec3 specular = specularStrength * spec * lightColor * attenuation;

        totalDiffuse += diffuse;
        totalSpecular += specular;
    }

    vec3 ambient = ambientStrength * vec3(1.0, 1.0, 1.0);
    vec3 result = (ambient + totalDiffuse + totalSpecular);

    out_color = vec4(result, 1.0) * texture(uTexture, frag_texCoords);
}