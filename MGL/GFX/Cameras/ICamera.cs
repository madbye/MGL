using System.Numerics;
using MGL.GFX.Shaders;

namespace MGL.GFX.Common;

public interface ICamera
{
    public Vector3 Position { get; set; }
    public Quaternion Rotation { get; set; }
    
    public Matrix4x4 GetViewMatrix()
    {
        Matrix4x4 rotation = Matrix4x4.CreateFromQuaternion(Rotation);
        Matrix4x4 translation = Matrix4x4.CreateTranslation(-Position);
        return translation * rotation;
    }
    
    public void LookAt(Vector3 target, Vector3? up = null)
    {
        up ??= Vector3.UnitY;
        
        Vector3 forward = Vector3.Normalize(target - Position);
        Vector3 right = Vector3.Normalize(Vector3.Cross((Vector3)up, forward));
        Vector3 actualUp = Vector3.Cross(forward, right);
        
        Rotation = Quaternion.CreateFromRotationMatrix(Matrix4x4.CreateLookAt(Position, target, actualUp));
    }

    public Matrix4x4 GetProjectionMatrix();
    public void SetVPMatricesToProgram(ShaderProgram program);

    public void SetPositionToProgram(ShaderProgram program)
    {
        program.SetUniform("viewPos", Position);
    }
}