using MGL.Common;
using Silk.NET.OpenGL;

namespace MGL.Graphics;

public class VertexArray : IDisposable
{
    public uint Handle { get; private set; }
    public uint VBO { get; private set; } 
    public uint EBO { get; private set; } 
    public uint IndicesCount { get; private set; }
    
    public VertexAttributePointer[] VertexLayout { get; set; }

    private Renderer _renderer;

    public unsafe VertexArray(Renderer renderer, float[] vertices, int[] indices, VertexAttributePointer[] vertexLayout, VertexArrayUsage vertexArrayUsage)
    {
        _renderer = renderer;
        
        BufferUsageARB bufferUsageArb;

        if (vertexArrayUsage == VertexArrayUsage.Static)
            bufferUsageArb = BufferUsageARB.StaticDraw;
        else 
            bufferUsageArb = BufferUsageARB.DynamicDraw;
        
        IndicesCount = (uint)indices.Length;

        this.VertexLayout = vertexLayout;

        Handle = _renderer.GlContext.GenVertexArray();
        _renderer.GlContext.BindVertexArray(Handle);

        VBO = _renderer.GlContext.GenBuffer();
        _renderer.GlContext.BindBuffer(BufferTargetARB.ArrayBuffer, VBO);
        fixed (float* buf = vertices)
            _renderer.GlContext.BufferData(BufferTargetARB.ArrayBuffer, (nuint)(vertices.Length * sizeof(float)), buf, bufferUsageArb);
        
        EBO = _renderer.GlContext.GenBuffer();
        _renderer.GlContext.BindBuffer(BufferTargetARB.ElementArrayBuffer, EBO);
        fixed (int* buf = indices)
            _renderer.GlContext.BufferData(BufferTargetARB.ElementArrayBuffer, (nuint)(indices.Length * sizeof(uint)), buf, bufferUsageArb);

        uint stride = (uint)vertexLayout.Sum(s => s.Size) * sizeof(float);
        
        foreach (var i in vertexLayout)
        {
            _renderer.GlContext.EnableVertexAttribArray(i.Location);
            _renderer.GlContext.VertexAttribPointer(i.Location, (int)i.Size, VertexAttribPointerType.Float, false, stride, (void*)(i.Offset * sizeof(float)));
        }
        
        _renderer.GlContext.BindVertexArray(0);
    }

    public static VertexArray FromCommonMesh(Renderer renderer, CommonMesh mesh)
    {
        List<float> vertices = new List<float>();
        foreach (var vertex in mesh.Vertices)
        {
            vertices.Add(vertex.Position.X);
            vertices.Add(vertex.Position.Y);
            vertices.Add(vertex.Position.Z);
            
            vertices.Add(vertex.Normal.X);
            vertices.Add(vertex.Normal.Y);
            vertices.Add(vertex.Normal.Z);
            
            vertices.Add(vertex.UV.X);
            vertices.Add(vertex.UV.Y);
        }

        return new(renderer, vertices.ToArray(), mesh.Indices.ToArray(), Graphics.VertexLayout.Default3D, VertexArrayUsage.Static);
    }
    
    public unsafe void Draw(PrimitiveType primitiveType = PrimitiveType.Triangles, PolygonFace polygonFace = PolygonFace.Front)
    {
        _renderer.GlContext.BindVertexArray(Handle);
        _renderer.GlContext.CullFace((TriangleFace)polygonFace);
        _renderer.GlContext.DrawElements((Silk.NET.OpenGL.PrimitiveType)primitiveType, IndicesCount, DrawElementsType.UnsignedInt,(void*) 0);
        _renderer.GlContext.BindVertexArray(0);
        _renderer.GlContext.CullFace(TriangleFace.Front);
    }
    
    public void Dispose()
    {
        _renderer.GlContext.DeleteVertexArray(Handle);
        _renderer.GlContext.DeleteBuffer(VBO);
        _renderer.GlContext.DeleteBuffer(EBO);
    }
}