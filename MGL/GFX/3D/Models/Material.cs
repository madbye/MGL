using System.Drawing;
using MGL.GFX.Textures;

namespace MGL.GFX._3D.Models;

public class Material
{
    public Color BaseColor { get; set; } = Color.White;
    
    public Texture2D? BaseColorMap { get; set; }
    public Texture2D? NormalMap { get; set; }
    public Texture2D? MetallicMap { get; set; }
    public Texture2D? RoughnessMap { get; set; }
    public Texture2D? AOMap { get; set; }

    public float RoughnessFactor { get; set; } = 1;
    public float MetallicFactor { get; set; } = 1;
}