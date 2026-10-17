using System.Numerics;
using MGL.Common;

namespace MGL.Physics;

public class MeshCollider : ISupportable
{
    public List<Vector3> Vertices { get; set; } = new List<Vector3>();
    public Matrix4x4 Transformation { get; set; } = Matrix4x4.Identity;

    public MeshCollider(CommonMesh commonMesh)
    {
        foreach (var vertex in commonMesh.Vertices)
        {
            Vertices.Add(vertex.Position);
        }
    }

    public MeshCollider(Vector3[] vertices)
    {
        Vertices.AddRange(vertices);
    }

    private (Matrix4x4, List<Vector3>) _cachedTransformatedVertices = new();
    private bool _cachedTransformatedVerticesInited = false;
    
    public List<Vector3> GetVerticesTransformed()
    {
        if (!_cachedTransformatedVerticesInited)
        {
            _cachedTransformatedVerticesInited = true;
        }
        else
        {
            if (Transformation == _cachedTransformatedVertices.Item1)
                return _cachedTransformatedVertices.Item2;
        }
        List<Vector3> transformatedVertices = new List<Vector3>();
        
        foreach (var vertex in Vertices)
        {
            transformatedVertices.Add(Vector3.Transform(vertex, Transformation));
        }

        _cachedTransformatedVertices.Item1 = Transformation;
        _cachedTransformatedVertices.Item2 = transformatedVertices;
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
}