using System.Numerics;

namespace MGL.Common;

public record Vertex(Vector3 position, Vector3 normal, Vector2 uv)
{
    public Vector3 Position { get; set; } = position;
    public Vector3 Normal { get; set; } = normal;
    public Vector2 UV { get; set; } = uv;
}