using MGL.GFX.Shaders;

namespace MGL.GFX._3D.Lights;

public static class Lighting
{
     public static void SetToProgram(Light[] lights, ShaderProgram program)
    {
        program.SetUniform("numLights", lights.Length);
        
        for (int i = 0; i < lights.Length; i++)
        {
            switch (lights[i])
            {
                case DirectionalLight dirLight:
                    program.SetUniform($"lights[{i}].type", 0);
                    program.SetUniform($"lights[{i}].direction", dirLight.Direction);
                    program.SetUniform($"lights[{i}].color", dirLight.Color);
                    break;
        
                case PointLight pointLight:
                    program.SetUniform($"lights[{i}].type", 1);
                    program.SetUniform($"lights[{i}].position", pointLight.Position);
                    program.SetUniform($"lights[{i}].color", pointLight.Color);
                    program.SetUniform($"lights[{i}].constant", pointLight.Constant);
                    program.SetUniform($"lights[{i}].linear", pointLight.Linear);
                    program.SetUniform($"lights[{i}].quadratic", pointLight.Quadratic);
                    program.SetUniform($"lights[{i}].radius", pointLight.Radius); // Если добавил радиус для отсечения
                    break;
        
                case SpotLight spotLight:
                    program.SetUniform($"lights[{i}].type", 2);
                    program.SetUniform($"lights[{i}].position", spotLight.Position);
                    program.SetUniform($"lights[{i}].direction", spotLight.Direction);
                    program.SetUniform($"lights[{i}].color", spotLight.Color);
                    program.SetUniform($"lights[{i}].constant", spotLight.Constant);
                    program.SetUniform($"lights[{i}].linear", spotLight.Linear);
                    program.SetUniform($"lights[{i}].quadratic", spotLight.Quadratic);
                    program.SetUniform($"lights[{i}].cutOff", spotLight.CutOff);
                    program.SetUniform($"lights[{i}].outerCutOff", spotLight.OuterCutOff);
                    program.SetUniform($"lights[{i}].radius", spotLight.Radius); // Тоже пригодится
                    break;
            }
        }
    }

    public static void SetAmbientStrength(float value, ShaderProgram shaderProgram)
    {
        shaderProgram.SetUniform("ambientStrength", 0.5f);
    }
}