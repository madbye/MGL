using System.Numerics;

namespace MGL.GFX._3D.Lights;

public class SpotLight : Light
{
    public Vector3 Position { get; set; }
    public Vector3 Direction { get; set; }
    public Vector3 Color { get; set; } = new Vector3(3);
    public float Constant { get; set; }
    public float Linear { get; set; }
    public float Quadratic { get; set; }
    public float CutOff { get; set; }
    public float OuterCutOff { get; set; }
    public float Radius { get; set; }
}