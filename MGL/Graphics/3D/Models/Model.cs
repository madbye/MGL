using System.Numerics;
using MGL.Physics;

namespace MGL.Graphics;

public class Model
{
    internal Renderer Renderer;
    public ModelNode Root { get; set; }
    public Material[] Materials { get; set; }

    public Model(ModelNode root, Material[] materials)
    {
        Root = root;
        Materials = materials;
    }

    public void Draw(Matrix4x4 transform)
    {
        Root.Draw(transform, Materials);
    }

    public List<MeshCollider> GetMeshColliders() => Root.GetMeshColliders(Renderer);
}