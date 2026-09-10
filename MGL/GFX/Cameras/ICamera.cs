using System.Numerics;

namespace MGL.GFX.Common;

public interface ICamera
{
    public static ICamera CurrentCamera { get; set; }
    
    public Vector3 Position { get; set; }
    public Quaternion Rotation { get; set; }
    
    public float Size { get; set; }
    
    public uint Width { get; set; }
    public uint Height { get; set; }
    
    public float NearPlaneDistance { get; set; }
    public float FarPlaneDistance { get; set; }
    
    public Matrix4x4 GetViewMatrix()
    {
        if (Matrix4x4.Invert(Matrix4x4.CreateFromQuaternion(Rotation) * Matrix4x4.CreateTranslation(Position), out Matrix4x4 viewMat))
        {
            return viewMat;
        }
        return Matrix4x4.Identity;
    }

    public Matrix4x4 GetProjectionMatrix();

}