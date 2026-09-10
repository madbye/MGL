using MGL.GFX.Shaders;

namespace MGL.GFX._3D.Models;

public class Mesh(VertexArray.VertexArray vertexArray, int materialIndex)
{
    public VertexArray.VertexArray VertexArray { get; set; } = vertexArray;
    public int MaterialIndex { get; set; } = materialIndex;

    public void Draw(Material[] materials)
    {
        var material = materials[MaterialIndex];
        
        if (material?.BaseColorMap != null)
            ShaderProgram.Current.SetUniformTextureUnit("colorMap", material.BaseColorMap,0);
        
        if (material?.NormalMap != null)
            ShaderProgram.Current.SetUniformTextureUnit("normalMap", material.NormalMap,1);
        
        if (material?.MetallicMap != null)
            ShaderProgram.Current.SetUniformTextureUnit("metallicMap", material.MetallicMap,2);
        
        if (material?.RoughnessMap != null)
            ShaderProgram.Current.SetUniformTextureUnit("roughnessMap", material.RoughnessMap,3);
        
        if (material?.AOMap != null)
            ShaderProgram.Current.SetUniformTextureUnit("aoMap", material.AOMap,4);
        
        VertexArray.Draw();
    }
}