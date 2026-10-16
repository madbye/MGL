namespace MGL.Common;

public class CommonMesh(Vertex[] vertices, int[] indices)
{
    public Vertex[] Vertices { get; set; } = vertices;
    public int[] Indices { get; set; } = indices;
}