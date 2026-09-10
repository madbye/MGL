using System.Numerics;
using MGL.GFX.Shaders;

namespace MGL.GFX.Common;

public class OrthoCamera : ICamera
{
    public Vector3 Position { get; set; }
    public Quaternion Rotation { get; set; }
    public float Size { get; set; }
    public uint Width { get; set; }
    public uint Height { get; set; }
    public float NearPlaneDistance { get; set; }
    public float FarPlaneDistance { get; set; }
    
    public OrthoCamera(Vector3 position, Quaternion rotation, uint width, uint height, float size, float near, float far)
    {
        Position = position;
        Rotation = rotation;
        Width = width;
        Height = height;
        Size = size;
        NearPlaneDistance = near;
        FarPlaneDistance = far;
    }
    
    public Matrix4x4 GetProjectionMatrix()
    {
        float aspectRatio = (float)Width / Height;
        float viewWidth = Size * aspectRatio;
        return Matrix4x4.CreateOrthographic(viewWidth, Size, NearPlaneDistance, FarPlaneDistance);
    }
    
    public void LookAt(Vector3 target, Vector3? up = null)
    {
        up ??= Vector3.UnitY;
        
        Matrix4x4 viewMatrix = Matrix4x4.CreateLookAt(Position, target, (Vector3)up);
        if (Matrix4x4.Invert(viewMatrix, out Matrix4x4 invertedMat))
        {
            Matrix4x4.Decompose(invertedMat, out _, out Quaternion rotation, out _);
            Rotation = rotation;
        }
    }

    public void SetMatricesToProgram(ShaderProgram program)
    {
        program.SetUniform("view", ((ICamera)this).GetViewMatrix());
        program.SetUniform("proj", GetProjectionMatrix());
    }
}