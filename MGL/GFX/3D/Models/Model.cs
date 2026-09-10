using System.Numerics;

namespace MGL.GFX._3D.Models;

public class Model
{
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
}