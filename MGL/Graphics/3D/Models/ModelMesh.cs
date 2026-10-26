using System.Numerics;
using MGL.Common;
using MGL.Physics;
using Silk.NET.OpenGL;

namespace MGL.Graphics;

public class ModelMesh(VertexArray vertexArray, int materialIndex)
{
    public VertexArray VertexArray { get; private set; } = vertexArray;
    public int MaterialIndex { get; private set; } = materialIndex;

    private MeshCollider? _cachedCollider;

    public MeshCollider GetMeshCollider(Renderer renderer)
    {
        if (_cachedCollider == null)
            _cachedCollider = GenMeshCollider(renderer);
        
        return _cachedCollider;
    }

    private MeshCollider GenMeshCollider(Renderer renderer)
    {
        List<Vector3> vertices = new List<Vector3>();
        List<int> indices = new List<int>();
        
        renderer.GlContext.BindBuffer(GLEnum.ArrayBuffer, VertexArray.VBO);
        renderer.GlContext.GetBufferParameter(GLEnum.ArrayBuffer, GLEnum.BufferSize, out int verticesBufferSize);
        
        int count = verticesBufferSize / sizeof(float);
        float[] verticesData = new float[count];
        
        renderer.GlContext.GetBufferSubData(GLEnum.ArrayBuffer, IntPtr.Zero, (nuint)verticesBufferSize, verticesData.AsSpan());

        for (int i = 0; i < count; i += 8)
        {
            vertices.Add(new(verticesData[i], verticesData[i + 1], verticesData[i + 2]));
        }
        
        renderer.GlContext.BindBuffer(GLEnum.ElementArrayBuffer, VertexArray.EBO);
        renderer.GlContext.GetBufferParameter(GLEnum.ElementArrayBuffer, GLEnum.BufferSize, out int indicesBufferSize);
        
        int[] indicesData = new int[indicesBufferSize / sizeof(int)];
        
        renderer.GlContext.GetBufferSubData(GLEnum.ElementArrayBuffer, IntPtr.Zero, (nuint)indicesBufferSize, indicesData.AsSpan());

        indices.AddRange(indicesData);

        return new(vertices.ToArray(), indices.ToArray());
    }
    
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