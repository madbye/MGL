namespace MGL.Graphics;

public enum ColorComponents
{
    R,
    RGB,
    RGBA
}
public class Image
{
    public byte[] PixelData { get; set; }
    public uint Width { get; set; }
    public uint Height { get; set; }
    public ColorComponents ColorComponents { get; set; }
    
    public Image(uint width, uint height, ColorComponents colorComponents, byte[] pixelData)
    {
        Width = width;
        Height = height;
        ColorComponents = colorComponents;
        PixelData = pixelData;
    }
}