using System.Drawing;
using Assimp;
using MGL.Graphics;
using MGL.Physics;
using Material = Assimp.Material;
using Matrix4x4 = System.Numerics.Matrix4x4;

namespace MGL.IO;

public static class ModelLoader
{
    public static Model LoadModel(Renderer renderer, string path)
    {
        using (var importer = new AssimpContext())
        {
            PostProcessSteps flags = PostProcessSteps.Triangulate | 
                                     PostProcessSteps.GenerateNormals;

            Scene scene = importer.ImportFile(path, flags);
            ModelNode root = ProcessNode(renderer, scene.RootNode, scene);
            Graphics.Material[] materials = new Graphics.Material[scene.MaterialCount];
            for (int i = 0; i < materials.Length; i++)
            {
                materials[i] = ProcessMaterial(renderer, scene.Materials[i], scene, Path.GetDirectoryName(path));
            }
            Model model = new(root, materials);
            model.Renderer = renderer;
            return model;
        }
    }

    private static Graphics.Material ProcessMaterial(Renderer renderer, Material material, Scene scene, string directoryPath)
    {
        var result = new Graphics.Material();
        if (material.HasColorDiffuse)
        {
            Color4D color4D = material.ColorDiffuse;
            result.BaseColor = Color.FromArgb(
                (byte)(color4D.A * 255),
                (byte)(color4D.R * 255),
                (byte)(color4D.G * 255),
                (byte)(color4D.B * 255)
            );
        }
        if (material.HasTextureDiffuse)
            result.BaseColorMap = ProcessTexture(renderer, material.TextureDiffuse, scene, directoryPath);
        if (material.HasTextureNormal)
            result.NormalMap = ProcessTexture(renderer, material.TextureNormal, scene, directoryPath);
        if (material.HasTextureAmbientOcclusion)
            result.AOMap = ProcessTexture(renderer, material.TextureAmbientOcclusion, scene, directoryPath);
        if (material.GetMaterialTextureCount(TextureType.Metalness) > 0)
        {
            TextureSlot slot; 
            material.GetMaterialTexture(TextureType.Metalness,0, out slot);
            result.MetallicMap = ProcessTexture(renderer, slot, scene, directoryPath);
        }
        if (material.GetMaterialTextureCount(TextureType.Roughness) > 0)
        {
            TextureSlot slot; 
            material.GetMaterialTexture(TextureType.Roughness,0, out slot);
            result.RoughnessMap = ProcessTexture(renderer, slot, scene, directoryPath);
        }

        return result;
    }

    private static Texture2D ProcessTexture(Renderer renderer, TextureSlot slot, Scene scene, string directoryPath)
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

        return Texture2D.FromImage(renderer, image, TextureFilter.Linear, true);
    }

    private static ModelNode ProcessNode(Renderer renderer, Node node, Scene scene)
    {
        var children = new List<ModelNode>();
        var meshes = new List<ModelMesh>();
        foreach (int meshIndex in node.MeshIndices)
        {
            Assimp.Mesh mesh = scene.Meshes[meshIndex];
            meshes.Add(ProcessMesh(renderer, mesh, scene));
        }
        foreach (var child in node.Children)
        {
            children.Add(ProcessNode(renderer, child, scene));
        }

        var m = node.Transform;
        var matrix = new Matrix4x4(m.A1, m.B1, m.C1, m.D1,
            m.A2, m.B2, m.C2, m.D2,
            m.A3, m.B3, m.C3, m.D3,
            m.A4, m.B4, m.C4, m.D4);
        
        
        return new ModelNode(node.Name, children, meshes, matrix);
    }

    private static ModelMesh ProcessMesh(Renderer renderer, Mesh mesh, Scene scene)
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

        VertexArray vertexArray = new VertexArray(renderer, vertices.ToArray(), mesh.GetIndices(), VertexLayout.Default3D, VertexArrayUsage.Static);
        int material = mesh.MaterialIndex;
        return new(vertexArray, material);
    }
}