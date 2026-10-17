using System.Numerics;
using Silk.NET.Core;

namespace MGL.Physics;

public static class GJK
{
    private const int MaxIterations = 64;
    private const float Epsilon = 1e-12f;

    public static bool Intersect(ISupportable a, ISupportable b) => Intersect(a, b, out _);

    public static bool Intersect(ISupportable a, ISupportable b, out List<Vector3> simplex)
    {
        Vector3 direction = Vector3.UnitX;
        Vector3 support = ComputeSupport(a, b, direction);
        
        simplex = new List<Vector3>(4) { support };

        direction = -support;
        if (direction.LengthSquared() < Epsilon)
        {
            return Inflate(a, b, simplex);
        }

        for (int i = 0; i < MaxIterations; i++)
        {
            support = ComputeSupport(a, b, direction);
            if (Vector3.Dot(support, direction) < 0)
            {
                return false;
            }

            simplex.Insert(0, support);
            if (UpdateSimplex(simplex, ref direction))
            {
                return true;
            }

            if (direction.LengthSquared() < Epsilon)
            {
                return Inflate(a, b, simplex);
            }
        }

        return false;
    }

    internal static Vector3 ComputeSupport(ISupportable a, ISupportable b, Vector3 direction) 
        =>  a.GetSupportPoint(direction) - b.GetSupportPoint(-direction);
    
    private static readonly Vector3[] InflateDirs =
    {
        Vector3.UnitX, -Vector3.UnitX,  Vector3.UnitY, -Vector3.UnitY,  Vector3.UnitZ, -Vector3.UnitZ 
    };
    
    private static bool Inflate(ISupportable a, ISupportable b, List<Vector3> s)
    {
        while (s.Count < 4)
        {
            bool added = false;
            foreach (var dir in InflateDirs)
            {
                Vector3 p = ComputeSupport(a, b, dir);
                bool independent = s.Count switch
                {
                    1 => (p - s[0]).LengthSquared() > Epsilon,
                    2 => Vector3.Cross(s[1] - s[0], p - s[0]).LengthSquared() > Epsilon,
                    3 => MathF.Abs(Vector3.Dot(Vector3.Cross(s[1] - s[0], s[2] - s[0]), p - s[0])) > Epsilon,
                    _ => false
                };
                if (independent)
                {
                    s.Add(p);
                    added = true;
                    break;
                }
            }

            if (!added) return true;
        }

        return true;
    }

    private static bool UpdateSimplex(List<Vector3> simplex, ref Vector3 direction)
    {
        return simplex.Count switch
        {
            2 => LineCase(simplex, ref direction),
            3 => TriangleCase(simplex, ref direction),
            4 => TetrahedronCase(simplex, ref direction),
            _ => false
        };
    }

    private static bool LineCase(List<Vector3> simplex, ref Vector3 direction)
    {
        Vector3 a = simplex[0];
        Vector3 b = simplex[1];

        Vector3 ab = b - a;
        Vector3 ao = -a;

        if (Vector3.Dot(ab, ao) > 0)
        {
            direction = Vector3.Cross(Vector3.Cross(ab, ao), ab);
        }
        else
        {
            simplex.Clear();
            simplex.Add(a);
            direction = ao;
        }

        return false;
    }

    private static bool TriangleCase(List<Vector3> simplex, ref Vector3 direction)
    {
        Vector3 a = simplex[0];
        Vector3 b = simplex[1];
        Vector3 c = simplex[2];

        Vector3 ab = b - a;
        Vector3 ac = c - a;
        Vector3 ao = -a;

        Vector3 abc = Vector3.Cross(ab, ac);

        if (Vector3.Dot(Vector3.Cross(abc, ac), ao) > 0)
        {
            if (Vector3.Dot(ac, ao) > 0)
            {
                simplex.Clear();
                simplex.Add(a);
                simplex.Add(c);
                direction = Vector3.Cross(Vector3.Cross(ac, ao), ac);
                return false;
            }

            simplex.Clear();
            simplex.Add(a);
            simplex.Add(b);
            return LineCase(simplex, ref direction);
        }

        if (Vector3.Dot(Vector3.Cross(ab, abc), ao) > 0)
        {
            simplex.Clear();
            simplex.Add(a);
            simplex.Add(b);
            return LineCase(simplex, ref direction);
        }

        if (Vector3.Dot(abc, ao) > 0)
        {
            direction = abc;
        }
        else
        {
            simplex.Clear();
            simplex.Add(a);
            simplex.Add(c);
            simplex.Add(b);
            direction = -abc;
        }

        return false;
    }

    private static bool TetrahedronCase(List<Vector3> simplex, ref Vector3 direction)
    {
        Vector3 a = simplex[0];
        Vector3 b = simplex[1];
        Vector3 c = simplex[2];
        Vector3 d = simplex[3];

        Vector3 ab = b - a;
        Vector3 ac = c - a;
        Vector3 ad = d - a;
        Vector3 ao = -a;

        Vector3 abc = Vector3.Cross(ab, ac);
        Vector3 acd = Vector3.Cross(ac, ad);
        Vector3 adb = Vector3.Cross(ad, ab);

        if (Vector3.Dot(abc, ao) > 0)
        {
            simplex.Clear();
            simplex.Add(a);
            simplex.Add(b);
            simplex.Add(c);
            return TriangleCase(simplex, ref direction);
        }

        if (Vector3.Dot(acd, ao) > 0)
        {
            simplex.Clear();
            simplex.Add(a);
            simplex.Add(c);
            simplex.Add(d);
            return TriangleCase(simplex, ref direction);
        }

        if (Vector3.Dot(adb, ao) > 0)
        {
            simplex.Clear();
            simplex.Add(a);
            simplex.Add(d);
            simplex.Add(b);
            return TriangleCase(simplex, ref direction);
        }

        return true;
    }
}