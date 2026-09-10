namespace MGL.GFX.VertexArray;

public static class VertexLayouts
{
    public static readonly VertexAttributePointer[] PosTex = new[]
    {
        new VertexAttributePointer(0, 3, 0),
        new VertexAttributePointer(1, 2, 3)
    };
    
    public static readonly VertexAttributePointer[] Default3D = new[]
    {
        new VertexAttributePointer(0, 3, 0),
        new VertexAttributePointer(1, 3, 3),
        new VertexAttributePointer(2, 2, 6)
    };
}