using System.Numerics;
using MGL.Physics;

namespace MGL.Graphics;

public class ModelNode(string name, List<ModelNode> nodes, List<ModelMesh> meshes, Matrix4x4 transform)
{
    public string Name { get; set; } = name;
    public List<ModelNode> Children { get; set; } = nodes;
    public List<ModelMesh> Meshes { get; set; } = meshes;
    public Matrix4x4 Transform { get; set; } = transform;

    public void Draw(Matrix4x4 transform, Material[] materials)
    {
        Matrix4x4 globalTransform = Transform * transform;
        
        if (ShaderProgram.Current.GetUniformLocation("model") != -1)
        {
            ShaderProgram.Current.SetUniform("model", globalTransform);
        }
        foreach (var mesh in Meshes)
        {
            mesh.Draw(materials);
        }

        foreach (var child in Children)
        {
            child.Draw(globalTransform, materials);
        }
    }

    public List<MeshCollider> GetMeshColliders(Renderer renderer)
    {
        List<MeshCollider> colliders = new List<MeshCollider>();
        foreach (var mesh in Meshes)
        {
            colliders.Add(mesh.GetMeshCollider(renderer));
        }
        foreach (var child in Children)
        {
            colliders.AddRange(child.GetMeshColliders(renderer));
        }

        return colliders;
    }
    public ModelNode? FindChild(string name)
    {
        foreach (var child in Children)
        {
            if (child.Name == name)
                return child;
        }
        foreach (var child in Children)
        {
            var found = child.FindChild(name);
            if (found != null)
                return found;
        }
        return null;
    }
}