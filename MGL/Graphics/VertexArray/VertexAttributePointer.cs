namespace MGL.Graphics;

public struct VertexAttributePointer(uint location, uint size, uint offset, VertexAttribPointerType type = VertexAttribPointerType.Float)
{
    public uint Location { get; } = location;
    public uint Size { get; } = size;
    public uint Offset { get; } = offset;
    public VertexAttribPointerType Type { get; } = type;
}