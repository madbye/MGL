using Silk.NET.OpenGL;

namespace MGL.GFX.VertexArray;

public class VertexArray : IDisposable
{
    public uint Handle { get; private set; }
    public uint VBO { get; private set; } 
    public uint EBO { get; private set; } 
    public uint IndicesCount { get; private set; }
    
    public VertexAttributePointer[] VertexLayout { get; set; }

    public unsafe VertexArray(float[] vertices, int[] indices, VertexAttributePointer[] vertexLayout)
    {
        IndicesCount = (uint)indices.Length;

        this.VertexLayout = vertexLayout;

        Handle = Env.Gl.GenVertexArray();
        Env.Gl.BindVertexArray(Handle);

        VBO = Env.Gl.GenBuffer();
        Env.Gl.BindBuffer(BufferTargetARB.ArrayBuffer, VBO);
        fixed (float* buf = vertices)
            Env.Gl.BufferData(BufferTargetARB.ArrayBuffer, (nuint)(vertices.Length * sizeof(float)), buf, BufferUsageARB.StaticDraw);
        
        EBO = Env.Gl.GenBuffer();
        Env.Gl.BindBuffer(BufferTargetARB.ElementArrayBuffer, EBO);
        fixed (int* buf = indices)
            Env.Gl.BufferData(BufferTargetARB.ElementArrayBuffer, (nuint)(indices.Length * sizeof(uint)), buf, BufferUsageARB.StaticDraw);

        uint stride = (uint)vertexLayout.Sum(s => s.Size) * sizeof(float);
        
        foreach (var i in vertexLayout)
        {
            Env.Gl.EnableVertexAttribArray(i.Location);
            Env.Gl.VertexAttribPointer(i.Location, (int)i.Size, VertexAttribPointerType.Float, false, stride, (void*)(i.Offset * sizeof(float)));
        }
        
        Env.Gl.BindVertexArray(0);
    }
    
    public unsafe void Draw(PrimitiveType primitiveType = PrimitiveType.Triangles, TriangleFace triangleFace = TriangleFace.Front)
    {
        Env.Gl.BindVertexArray(Handle);
        Env.Gl.CullFace(triangleFace);
        Env.Gl.DrawElements(primitiveType, IndicesCount, DrawElementsType.UnsignedInt,(void*) 0);
        Env.Gl.BindVertexArray(0);
        Env.Gl.CullFace(TriangleFace.Front);
    }
    
    public void Dispose()
    {
        Env.Gl.DeleteVertexArray(Handle);
        Env.Gl.DeleteBuffer(VBO);
        Env.Gl.DeleteBuffer(EBO);
    }
}