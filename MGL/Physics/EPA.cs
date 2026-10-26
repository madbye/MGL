using System.Numerics;

namespace MGL.Physics;

public static class EPA
{
    private const int MaxIterations = 64;
    private const float Tolerance = 1e-4f;

    private struct Face
    {
        public int A, B, C;
        public Vector3 Normal;
        public float Distance;
    }

    public static bool TryGetPenetration(
        ICollider a, ICollider b,
        IReadOnlyList<Vector3> simplex,
        out Vector3 normal, out float depth)
    {
        normal = default;
        depth = 0;

        if (simplex.Count != 4)
            return false;

        var verts = new List<Vector3>(simplex);
        var faces = new List<Face>(16);
        
        Vector3 inside = (verts[0] + verts[1] + verts[2] + verts[3]) / 4f;

        if (!AddFace(faces, verts, 0, 1, 2, inside) ||
            !AddFace(faces, verts, 0, 3, 1, inside) ||
            !AddFace(faces, verts, 0, 2, 3, inside) ||
            !AddFace(faces, verts, 1, 3, 2, inside))
            return false;

        var edges = new List<(int, int)>();

        for (int iter = 0; iter < MaxIterations; iter++)
        {
            int closest = 0;
            for (int i = 1; i < faces.Count; i++)
                if (faces[i].Distance < faces[closest].Distance)
                    closest = i;

            Face f = faces[closest];

            Vector3 p = GJK.ComputeSupport(a, b, f.Normal);
            float dist = Vector3.Dot(f.Normal, p);

            if (dist - f.Distance < Tolerance)
            {
                normal = f.Normal;
                depth = f.Distance;
                return true;
            }

            edges.Clear();
            for (int i = faces.Count - 1; i >= 0; i--)
            {
                Face face = faces[i];
                if (Vector3.Dot(face.Normal, p - verts[face.A]) > 0)
                {
                    AddEdge(edges, face.A, face.B);
                    AddEdge(edges, face.B, face.C);
                    AddEdge(edges, face.C, face.A);
                    faces.RemoveAt(i);
                }
            }

            int pi = verts.Count;
            verts.Add(p);
            foreach (var (e0, e1) in edges)
                AddFace(faces, verts, e0, e1, pi, inside);

            if (faces.Count == 0)
                return false;
        }

        int best = 0;
        for (int i = 1; i < faces.Count; i++)
            if (faces[i].Distance < faces[best].Distance)
                best = i;

        normal = faces[best].Normal;
        depth = faces[best].Distance;
        return true;
    }

    private static bool AddFace(List<Face> faces, List<Vector3> v, int a, int b, int c, Vector3 inside)
    {
        Vector3 n = Vector3.Cross(v[b] - v[a], v[c] - v[a]);
        float len = n.Length();

        if (len < 1e-8f)
            return false;

        n /= len;
        
        if (Vector3.Dot(n, v[a] - inside) < 0)
        {
            (b, c) = (c, b);
            n = -n;
        }

        float d = MathF.Max(0f, Vector3.Dot(n, v[a]));
        faces.Add(new Face { A = a, B = b, C = c, Normal = n, Distance = d });
        return true;
    }

    private static void AddEdge(List<(int, int)> edges, int a, int b)
    {
        int idx = edges.IndexOf((b, a));
        if (idx >= 0)
            edges.RemoveAt(idx);
        else
            edges.Add((a, b));
    }
}