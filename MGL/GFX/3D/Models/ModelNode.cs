using System.Numerics;
using MGL.GFX.Shaders;

namespace MGL.GFX._3D.Models;

public class ModelNode(string name, List<ModelNode> nodes, List<Mesh> meshes, Matrix4x4 transform)
{
    public string Name { get; set; } = name;
    public List<ModelNode> Children { get; set; } = nodes;
    public List<Mesh> Meshes { get; set; } = meshes;
    public Matrix4x4 Transform { get; set; } = transform;

    public void Draw(Matrix4x4 transform, Material[] materials)
    {
        Matrix4x4 globalTransform = Transform * transform;
        if(ShaderProgram.Current.GetUniformLocation("model") != -1)
            ShaderProgram.Current.SetUniform("model", globalTransform);
        foreach (var mesh in Meshes)
        {
            mesh.Draw(materials);
        }

        foreach (var child in Children)
        {
            child.Draw(globalTransform, materials);
        }
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