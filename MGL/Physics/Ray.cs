using System.Numerics;

namespace MGL.Physics;

public struct RaycastResult
{
    public bool Hit;
    public float Distance;
    public Vector3 Point;
    public Vector3 Normal;
}
public class Ray
{
    public Vector3 Origin { get; set; }
    public Vector3 Direction { get; set; }
}