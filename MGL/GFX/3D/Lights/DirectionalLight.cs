using System.Numerics;

namespace MGL.GFX._3D.Lights;

public class DirectionalLight : Light
{
    public Vector3 Direction { get; set; } = new(0, -1, 0);
    public Vector3 Color { get; set; } = new(1.0f, 1.0f, 1.0f); // Яркий белый свет
}