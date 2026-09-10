using System.Numerics;

namespace MGL.Utils;

public static class Easings
{
    public static float Interpolate(float a, float b, float n) { return a + n * (b - a); }
    public static Vector2 Interpolate(Vector2 a, Vector2 b, float n) { return a + n * (b - a); }
    public static Vector3 Interpolate(Vector3 a, Vector3 b, float n) { return a + n * (b - a); }
    public static Matrix4x4 Interpolate(Matrix4x4 a, Matrix4x4 b, float n) { return a + (b - a) * n; }
    
    // Expo
    public static double EaseInExpo(double n) { return n == 0 ? 0 : Math.Pow(2, 10 * n - 10 ); }
    public static double EaseOutExpo(double n) { return n == 1 ? 1 : 1 - Math.Pow(2, -10 * n); }
    public static double EaseInOutExpo(double n)
    {
        return n == 0 
            ? 0 
            : n == 1
                ? 1
                : n < 0.5 ? Math.Pow(2, 20 * n - 10) / 2 
                    : (2 - Math.Pow(2, -20 * n + 10)) / 2;
    }

    public static double ParabolicArc(double n)
    {
        return 4f * n * (1f - n);
    }
}