using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Numerics;
using Assimp;
using MGL.GFX;
using MGL.GFX._3D.Models;
using MGL.GFX.Common;
using MGL.GFX.Textures;
using MGL.GFX.VertexArray;
using Material = Assimp.Material;
using Matrix4x4 = System.Numerics.Matrix4x4;
using Mesh = MGL.GFX._3D.Models.Mesh;

namespace MGL.IO;

public static class ModelLoader
{
    private static AssimpContext _assimp = new();

    public static Model LoadModel(string path)
    {
        using (var importer = new AssimpContext())
        {
            PostProcessSteps flags = PostProcessSteps.Triangulate | 
                                     PostProcessSteps.GenerateNormals;

            Scene scene = importer.ImportFile(path, flags);
            ModelNode root = ProcessNode(scene.RootNode, scene);
            GFX._3D.Models.Material[] materials = new GFX._3D.Models.Material[scene.MaterialCount];
            for (int i = 0; i < materials.Length; i++)
            {
                materials[i] = ProcessMaterial(scene.Materials[i], scene, Path.GetDirectoryName(path));
            }
            return new(root, materials);
        }
    }

    private static GFX._3D.Models.Material ProcessMaterial(Material material, Scene scene, string directoryPath)
    {
        var result = new GFX._3D.Models.Material();
        if (material.HasColorDiffuse)
        {
            Color4D сolor4d = material.ColorDiffuse;
            result.BaseColor = Color.FromArgb(
                (byte)(сolor4d.A * 255),
                (byte)(сolor4d.R * 255),
                (byte)(сolor4d.G * 255),
                (byte)(сolor4d.B * 255)
            );
        }
        if (material.HasTextureDiffuse)
            result.BaseColorMap = ProcessTexture(material.TextureDiffuse, scene, directoryPath);
        if (material.HasTextureNormal)
            result.NormalMap = ProcessTexture(material.TextureNormal, scene, directoryPath);
        if (material.HasTextureAmbientOcclusion)
            result.AOMap = ProcessTexture(material.TextureAmbientOcclusion, scene, directoryPath);
        if (material.GetMaterialTextureCount(TextureType.Metalness) > 0)
        {
            TextureSlot slot; 
            material.GetMaterialTexture(TextureType.Metalness,0, out slot);
            result.MetallicMap = ProcessTexture(slot, scene, directoryPath);
        }
        if (material.GetMaterialTextureCount(TextureType.Roughness) > 0)
        {
            TextureSlot slot; 
            material.GetMaterialTexture(TextureType.Roughness,0, out slot);
            result.RoughnessMap = ProcessTexture(slot, scene, directoryPath);
        }

        return result;
    }

    private static Texture2D ProcessTexture(TextureSlot slot, Scene scene, string directoryPath)
    {
        string path = slot.FilePath;
        
        Image image;

        if (path.StartsWith("*"))
        {
            int index = int.Parse(path.Substring(1));
            var embeddedTex = scene.Textures[index];
            
            using (MemoryStream stream = new MemoryStream(embeddedTex.CompressedData))
            {
                image = ImageLoader.LoadImage(stream, ColorComponents.RGBA);
            }
        }
        else
        {
            string fullPath = Path.Combine(directoryPath, path);
            image = ImageLoader.LoadImage(fullPath, ColorComponents.RGBA);
        }

        return Texture2D.FromImage(image, TextureFilter.Linear, true);
    }

    private static ModelNode ProcessNode(Node node, Scene scene)
    {
        var children = new List<ModelNode>();
        var meshes = new List<Mesh>();
        foreach (int meshIndex in node.MeshIndices)
        {
            Assimp.Mesh mesh = scene.Meshes[meshIndex];
            meshes.Add(ProcessMesh(mesh, scene));
        }
        foreach (var child in node.Children)
        {
            children.Add(ProcessNode(child, scene));
        }

        var m = node.Transform;
        var matrix = new Matrix4x4(m.A1, m.A2, m.A3, m.A4,
                                   m.B1, m.B2, m.B3, m.B4,
                                   m.C1, m.C2, m.C3, m.C4,
                                   m.D1, m.D2, m.D3, m.D4);
        
        
        return new ModelNode(node.Name, children, meshes, matrix);
    }

    private static Mesh ProcessMesh(Assimp.Mesh mesh, Scene scene)
    {
        var vertices = new List<float>();
        for (int i = 0; i < mesh.VertexCount; i++)
        {
             vertices.Add(mesh.Vertices[i].X);
             vertices.Add(mesh.Vertices[i].Y);
             vertices.Add(mesh.Vertices[i].Z);

             if (mesh.HasNormals)
             {
                 var n = mesh.Normals[i];
                 vertices.Add(n.X);
                 vertices.Add(n.Y);
                 vertices.Add(n.Z);
             }
             else
             {
                 vertices.Add(0.0f);
                 vertices.Add(0.0f);
                 vertices.Add(0.0f);
             }
             
             if (mesh.HasTextureCoords(0))
             {
                 var uv = mesh.TextureCoordinateChannels[0][i];
                 vertices.Add(uv.X);
                 vertices.Add(uv.Y); 
             }
             else
             {
                 vertices.Add(0.0f);
                 vertices.Add(0.0f);
             }
        }

        VertexArray vertexArray = new VertexArray(vertices.ToArray(), mesh.GetIndices(), VertexLayouts.Default3D);
        int material = mesh.MaterialIndex;
        return new(vertexArray, material);
    }
}