using System.Drawing;

namespace MGL.Graphics;

public class Material
{
    public Color? BaseColor { get; set; }
    
    public Texture2D? BaseColorMap { get; set; }
    public Texture2D? NormalMap { get; set; }
    public Texture2D? MetallicMap { get; set; }
    public Texture2D? RoughnessMap { get; set; }
    public Texture2D? AOMap { get; set; }
}