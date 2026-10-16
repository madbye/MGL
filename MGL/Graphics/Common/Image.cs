namespace MGL.Graphics;

public class Image
{
    public byte[] PixelData { get; set; }
    public uint Width { get; set; }
    public uint Height { get; set; }
    public byte Channels { get; set; }
    
    public Image(uint width, uint height, byte channels, byte[] pixelData)
    {
        Width = width;
        Height = height;
        Channels = channels;
        PixelData = pixelData;
    }
}