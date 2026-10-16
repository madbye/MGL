using System.Drawing;
using System.IO;
using MGL.Graphics;
using MGL.Graphics;
using StbImageSharp;

namespace MGL.IO;

public enum ColorComponents
{
    R,
    RGB,
    RGBA
}
public static class ImageLoader
{
    public static Image LoadImage(Stream stream, ColorComponents colorComponents)
    {
        StbImage.stbi_set_flip_vertically_on_load(1);
        var (cc, channels) = GetStbColorComponents(colorComponents);
        
        ImageResult result = ImageResult.FromStream(stream, cc);
        return new Image((uint)result.Width, (uint)result.Height, channels, result.Data);
    }
    
    public static Image LoadImage(string path, ColorComponents colorComponents)
    {
        using (FileStream stream = File.OpenRead(path))
        {
            return LoadImage(stream, colorComponents);
        }
    }
    
    private static (StbImageSharp.ColorComponents cc, byte channels) GetStbColorComponents(ColorComponents colorComponents)
    {
        return colorComponents switch
        {
            ColorComponents.R    => (StbImageSharp.ColorComponents.Grey, 1),
            ColorComponents.RGB  => (StbImageSharp.ColorComponents.RedGreenBlue, 3),
            ColorComponents.RGBA => (StbImageSharp.ColorComponents.RedGreenBlueAlpha, 4),
            _ => throw new System.ArgumentOutOfRangeException(nameof(colorComponents))
        };
    }
}