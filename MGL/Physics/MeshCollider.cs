using System.Numerics;
using MGL.Common;

namespace MGL.Physics;

public class MeshCollider : ICollider
{
    public List<Vector3> Vertices { get; set; } = new List<Vector3>();
    public List<int> Indices { get; set; } = new List<int>();
    public Matrix4x4 Transformation { get; set; } = Matrix4x4.Identity;

    public MeshCollider(CommonMesh commonMesh)
    {
        foreach (var vertex in commonMesh.Vertices)
        {
            Vertices.Add(vertex.Position);
        }

        Indices.AddRange(commonMesh.Indices);
    }

    public MeshCollider(Vector3[] vertices, int[] indices)
    {
        Vertices.AddRange(vertices);
        Indices.AddRange(indices);
    }

    private (Matrix4x4, List<Vector3>) _cachedTransformedVertices = new();
    private bool _cachedTransformedVerticesInited = false;
    
    public List<Vector3> GetVerticesTransformed()
    {
        if (!_cachedTransformedVerticesInited)
        {
            _cachedTransformedVerticesInited = true;
        }
        else
        {
            if (Transformation == _cachedTransformedVertices.Item1)
                return _cachedTransformedVertices.Item2;
        }
        List<Vector3> transformatedVertices = new List<Vector3>();
        
        foreach (var vertex in Vertices)
        {
            transformatedVertices.Add(Vector3.Transform(vertex, Transformation));
        }

        _cachedTransformedVertices.Item1 = Transformation;
        _cachedTransformedVertices.Item2 = transformatedVertices;
        return transformatedVertices;
    }

    public Vector3 GetSupportPoint(Vector3 direction)
    {
        if (Vertices.Count == 0)
            return Vector3.Zero;

        List<Vector3> vertices = GetVerticesTransformed();

        float maxDot = float.MinValue;
        Vector3 bestVertex = vertices[0];

        for (int i = 0; i < vertices.Count; i++)
        {
            float dot = Vector3.Dot(vertices[i], direction);
            if (dot > maxDot)
            {
                maxDot = dot;
                bestVertex = vertices[i];
            }
        }

        return bestVertex;
    }
    
    public RaycastResult CheckRayIntersection(Ray ray)
    {
        bool hit = false;
        float closestDistance = float.MaxValue;
        Vector3 bestHitPoint = Vector3.Zero;
        Vector3 bestNormal = Vector3.Zero;
        
        for (int i = 0; i < Indices.Count; i += 3)
        {
            Vector3 v0 = Vertices[Indices[i]];
            Vector3 v1 = Vertices[Indices[i + 1]];
            Vector3 v2 = Vertices[Indices[i + 2]];

            RaycastResult result = CheckRayTriangleIntersection(ray, v0, v1, v2);

            if (result.Hit && result.Distance < closestDistance)
            {
                closestDistance = result.Distance;
                hit = true;
                bestHitPoint = result.Point;
                bestNormal = result.Normal;
            }
        }

        if (!hit)
        {
            return new RaycastResult { Hit = false };
        }

        return new RaycastResult
        {
            Hit = true,
            Distance = closestDistance,
            Point = bestHitPoint,
            Normal = bestNormal
        };
    }

    private RaycastResult CheckRayTriangleIntersection(Ray ray, Vector3 v0, Vector3 v1, Vector3 v2)
    {
        Vector3 edge1 = v1 - v0;
        Vector3 edge2 = v2 - v0;

        Vector3 h = Vector3.Cross(ray.Direction, edge2);
        float a = Vector3.Dot(edge1, h);

        if (a > -1e-7f && a < 1e-7f)
        {
            return new RaycastResult { Hit = false };
        }

        float f = 1f / a;
        Vector3 s = ray.Origin - v0;
        float u = f * Vector3.Dot(s, h);

        if (u < 0.0f || u > 1.0f)
        {
            return new RaycastResult { Hit = false };
        }

        Vector3 q = Vector3.Cross(s, edge1);
        float v = f * Vector3.Dot(ray.Direction, q);

        if (v < 0.0f || u + v > 1.0f)
        {
            return new RaycastResult { Hit = false };
        }

        float t = f * Vector3.Dot(edge2, q);

        if (t > 1e-7f)
        {
            Vector3 hitPoint = ray.Origin + ray.Direction * t;
            
            Vector3 normal = Vector3.Normalize(Vector3.Cross(edge1, edge2));

            return new RaycastResult
            {
                Hit = true,
                Distance = t,
                Point = hitPoint,
                Normal = normal
            };
        }

        return new RaycastResult { Hit = false };
    }
}