using System.Numerics;
using MGL.Common;

namespace MGL.Utils;

public static class MeshGenerator
{
    public static CommonMesh GenQuad(float width = 1f, float height = 1f)
    {
        Vertex[] vertices =
        {
            new Vertex(new(-width, -height, 0f), new Vector3(0f, 0f, 1f), new Vector2(0f, 0f)),
            new Vertex(new( width, -height, 0f), new Vector3(0f, 0f, 1f), new Vector2(1f, 0f)),
            new Vertex(new( width,  height, 0f), new Vector3(0f, 0f, 1f), new Vector2(1f, 1f)),
            new Vertex(new(-width,  height, 0f), new Vector3(0f, 0f, 1f), new Vector2(0f, 1f))
        };
        int[] indices =
        {
            0, 1, 3,
            1, 2, 3
        };
        return new(vertices, indices);
    }
    
    public static CommonMesh GenCube()
    {
        Vertex[] vertices =
        {
            // Front face
            new Vertex(new(-0.5f, -0.5f,  0.5f), new Vector3(0f, 0f, 1f), new Vector2(0f, 0f)),
            new Vertex(new( 0.5f, -0.5f,  0.5f), new Vector3(0f, 0f, 1f), new Vector2(1f, 0f)),
            new Vertex(new( 0.5f,  0.5f,  0.5f), new Vector3(0f, 0f, 1f), new Vector2(1f, 1f)),
            new Vertex(new(-0.5f,  0.5f,  0.5f), new Vector3(0f, 0f, 1f), new Vector2(0f, 1f)),

            // Back face
            new Vertex(new(-0.5f, -0.5f, -0.5f), new Vector3(0f, 0f, -1f), new Vector2(1f, 0f)),
            new Vertex(new( 0.5f, -0.5f, -0.5f), new Vector3(0f, 0f, -1f), new Vector2(0f, 0f)),
            new Vertex(new( 0.5f,  0.5f, -0.5f), new Vector3(0f, 0f, -1f), new Vector2(0f, 1f)),
            new Vertex(new(-0.5f,  0.5f, -0.5f), new Vector3(0f, 0f, -1f), new Vector2(1f, 1f)),

            // Top face
            new Vertex(new(-0.5f,  0.5f, -0.5f), new Vector3(0f, 1f, 0f), new Vector2(0f, 1f)),
            new Vertex(new( 0.5f,  0.5f, -0.5f), new Vector3(0f, 1f, 0f), new Vector2(1f, 1f)),
            new Vertex(new( 0.5f,  0.5f,  0.5f), new Vector3(0f, 1f, 0f), new Vector2(1f, 0f)),
            new Vertex(new(-0.5f,  0.5f,  0.5f), new Vector3(0f, 1f, 0f), new Vector2(0f, 0f)),

            // Bottom face
            new Vertex(new(-0.5f, -0.5f, -0.5f), new Vector3(0f, -1f, 0f), new Vector2(0f, 0f)),
            new Vertex(new( 0.5f, -0.5f, -0.5f), new Vector3(0f, -1f, 0f), new Vector2(1f, 0f)),
            new Vertex(new( 0.5f, -0.5f,  0.5f), new Vector3(0f, -1f, 0f), new Vector2(1f, 1f)),
            new Vertex(new(-0.5f, -0.5f,  0.5f), new Vector3(0f, -1f, 0f), new Vector2(0f, 1f)),

            // Right face
            new Vertex(new( 0.5f, -0.5f, -0.5f), new Vector3(1f, 0f, 0f), new Vector2(1f, 0f)),
            new Vertex(new( 0.5f, -0.5f,  0.5f), new Vector3(1f, 0f, 0f), new Vector2(0f, 0f)),
            new Vertex(new( 0.5f,  0.5f,  0.5f), new Vector3(1f, 0f, 0f), new Vector2(0f, 1f)),
            new Vertex(new( 0.5f,  0.5f, -0.5f), new Vector3(1f, 0f, 0f), new Vector2(1f, 1f)),

            // Left face
            new Vertex(new(-0.5f, -0.5f, -0.5f), new Vector3(-1f, 0f, 0f), new Vector2(0f, 0f)),
            new Vertex(new(-0.5f, -0.5f,  0.5f), new Vector3(-1f, 0f, 0f), new Vector2(1f, 0f)),
            new Vertex(new(-0.5f,  0.5f,  0.5f), new Vector3(-1f, 0f, 0f), new Vector2(1f, 1f)),
            new Vertex(new(-0.5f,  0.5f, -0.5f), new Vector3(-1f, 0f, 0f), new Vector2(0f, 1f))
        };

        int[] indices =
        {
            0, 1, 2,  2, 3, 0,    
            4, 5, 6,  6, 7, 4,    
            8, 9, 10, 10, 11, 8,  
            12, 13, 14, 14, 15, 12,
            16, 17, 18, 18, 19, 16,
            20, 21, 22, 22, 23, 20 
        };

        return new(vertices, indices);
    }

    public static CommonMesh GenUVSphere(uint stackCount, uint sectorCount)
    {
        var vertices = new List<Vertex>();
    
        for (int i = 0; i <= stackCount; ++i)
        {
            float stackAngle = MathF.PI / 2 - i * (MathF.PI / stackCount); 
            float xy = MathF.Cos(stackAngle);
            float y = MathF.Sin(stackAngle);

            for (int j = 0; j <= sectorCount; ++j)
            {
                float sectorAngle = j * (2 * MathF.PI / sectorCount); 
            
                float x = xy * MathF.Cos(sectorAngle);
                float z = xy * MathF.Sin(sectorAngle); 
            
                Vector3 position = new(x, y, z);
                Vector3 normal = new(x, y, z);
                Vector2 uv = new((float)j / sectorCount, (float)i / stackCount);

                vertices.Add(new Vertex(position, normal, uv));
            }
        }

        var indices = new List<int>();
    
        for (uint i = 0; i < stackCount; ++i) 
        {
            uint k1 = i * (sectorCount + 1);     
            uint k2 = k1 + (sectorCount + 1);    

            for (int j = 0; j < sectorCount; ++j, ++k1, ++k2)
            {
                if (i != 0)
                {
                    indices.Add((int)k1);
                    indices.Add((int)k2);
                    indices.Add((int)k1 + 1);
                }
            
                if (i != (stackCount - 1))
                { 
                    indices.Add((int)k1 + 1);
                    indices.Add((int)k2);
                    indices.Add((int)k2 + 1);
                }
            }
        }

        return new(vertices.ToArray(), indices.ToArray());
    }

    public static CommonMesh GenPlane(float size = 1f, ushort textureTiling = 1)
    {
        float halfSize = size / 2f;
        Vertex[] vertices =
        {
            new Vertex(new( halfSize, 0f,  halfSize), new Vector3(0f, 1f, 0f), new Vector2(-textureTiling, 0f)),
            new Vertex(new( halfSize, 0f, -halfSize), new Vector3(0f, 1f, 0f), new Vector2(-textureTiling, -textureTiling)),
            new Vertex(new(-halfSize, 0f, -halfSize), new Vector3(0f, 1f, 0f), new Vector2(0f, -textureTiling)),
            new Vertex(new(-halfSize, 0f,  halfSize), new Vector3(0f, 1f, 0f), new Vector2(0f, 0f))
        };
        
        int[] indices =
        {
            0, 1, 3,
            1, 2, 3
        };
        
        return new(vertices, indices);
    }
}