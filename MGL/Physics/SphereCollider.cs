using System.Numerics;

namespace MGL.Physics;

public class SphereCollider : ICollider
{
    public Vector3 Center { get; set; }
    public float Radius { get; set; }

    public SphereCollider(Vector3 center, float radius)
    {
        Center = center;
        Radius = radius;
    }

    public Vector3 GetSupportPoint(Vector3 direction)
    {
        return Center + direction * Radius;
    }

    public RaycastResult CheckRayIntersection(Ray ray)
    {
        Vector3 oc = ray.Origin - Center;
        
        float a = Vector3.Dot(ray.Direction, ray.Direction);
        float b = 2.0f * Vector3.Dot(oc, ray.Direction);
        float c = Vector3.Dot(oc, oc) - Radius * Radius;

        float discriminant = b * b - 4.0f * a * c;

        if (discriminant < 0.0f)
        {
            return new RaycastResult { Hit = false };
        }

        float sqrtDiscriminant = MathF.Sqrt(discriminant);
        float t0 = (-b - sqrtDiscriminant) / (2.0f * a);
        float t1 = (-b + sqrtDiscriminant) / (2.0f * a);

        float t = t0;
        if (t < 0.0f)
        {
            t = t1;
            if (t < 0.0f)
            {
                return new RaycastResult { Hit = false };
            }
        }
        
        Vector3 hitPoint = ray.Origin + ray.Direction * t;
        Vector3 normal = Vector3.Normalize(hitPoint - Center);

        return new RaycastResult
        {
            Hit = true,
            Distance = t,
            Point = hitPoint,
            Normal = normal
        };
    }
}