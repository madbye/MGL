using System.Drawing;
using System.Numerics;

namespace MGL.GFX._3D.Lights;

public class PointLight : Light
{
    public Vector3 Position { get; set; }
    public Color Color { get; set; }
    public float Constant { get; set; }
    public float Linear { get; set; }
    public float Quadratic { get; set; }
    public float Radius { get; set; }
}