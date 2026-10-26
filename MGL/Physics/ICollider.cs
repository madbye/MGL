using System.Numerics;

namespace MGL.Physics;

public interface ICollider
{
    public Vector3 GetSupportPoint(Vector3 direction);
    public RaycastResult CheckRayIntersection(Ray ray);
}