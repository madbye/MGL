namespace MGL.GFX.VertexArray;

public readonly struct VertexAttributePointer(uint location, uint size, uint offset)
{
    public uint Location { get; } = location;
    public uint Size { get; } = size;
    public uint Offset { get; } = offset;
}