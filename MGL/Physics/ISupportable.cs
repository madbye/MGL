using System.Numerics;

namespace MGL.Physics;

public interface ISupportable
{
    Vector3 GetSupportPoint(Vector3 direction);
}