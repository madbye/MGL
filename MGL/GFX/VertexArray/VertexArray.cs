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

        Handle = Window.Current.GlContext.GenVertexArray();
        Window.Current.GlContext.BindVertexArray(Handle);

        VBO = Window.Current.GlContext.GenBuffer();
        Window.Current.GlContext.BindBuffer(BufferTargetARB.ArrayBuffer, VBO);
        fixed (float* buf = vertices)
            Window.Current.GlContext.BufferData(BufferTargetARB.ArrayBuffer, (nuint)(vertices.Length * sizeof(float)), buf, BufferUsageARB.StaticDraw);
        
        EBO = Window.Current.GlContext.GenBuffer();
        Window.Current.GlContext.BindBuffer(BufferTargetARB.ElementArrayBuffer, EBO);
        fixed (int* buf = indices)
            Window.Current.GlContext.BufferData(BufferTargetARB.ElementArrayBuffer, (nuint)(indices.Length * sizeof(uint)), buf, BufferUsageARB.StaticDraw);

        uint stride = (uint)vertexLayout.Sum(s => s.Size) * sizeof(float);
        
        foreach (var i in vertexLayout)
        {
            Window.Current.GlContext.EnableVertexAttribArray(i.Location);
            Window.Current.GlContext.VertexAttribPointer(i.Location, (int)i.Size, VertexAttribPointerType.Float, false, stride, (void*)(i.Offset * sizeof(float)));
        }
        
        Window.Current.GlContext.BindVertexArray(0);
    }
    
    public unsafe void Draw(PrimitiveType primitiveType = PrimitiveType.Triangles, TriangleFace triangleFace = TriangleFace.Front)
    {
        Window.Current.GlContext.BindVertexArray(Handle);
        Window.Current.GlContext.CullFace(triangleFace);
        Window.Current.GlContext.DrawElements(primitiveType, IndicesCount, DrawElementsType.UnsignedInt,(void*) 0);
        Window.Current.GlContext.BindVertexArray(0);
        Window.Current.GlContext.CullFace(TriangleFace.Front);
    }
    
    public void Dispose()
    {
        Window.Current.GlContext.DeleteVertexArray(Handle);
        Window.Current.GlContext.DeleteBuffer(VBO);
        Window.Current.GlContext.DeleteBuffer(EBO);
    }
}