using MGL.Graphics;
using StbImageSharp;
using ColorComponents = MGL.Graphics.ColorComponents;


namespace MGL.IO;

public static class ImageLoader
{
    public static Image LoadImage(Stream stream, ColorComponents colorComponents, bool flipVertically = true)
    {
        StbImage.stbi_set_flip_vertically_on_load(flipVertically ? 1 : 0);
        var cc = GetStbColorComponents(colorComponents);
        
        ImageResult result = ImageResult.FromStream(stream, cc);
        return new Image((uint)result.Width, (uint)result.Height, colorComponents, result.Data);
    }
    
    public static Image LoadImage(string path, ColorComponents colorComponents, bool flipVertically = true)
    {
        using (FileStream stream = File.OpenRead(path))
            return LoadImage(stream, colorComponents, flipVertically);
    }
    
    private static StbImageSharp.ColorComponents GetStbColorComponents(ColorComponents colorComponents)
    {
        return colorComponents switch
        {
            ColorComponents.R    => StbImageSharp.ColorComponents.Grey,
            ColorComponents.RGB  => StbImageSharp.ColorComponents.RedGreenBlue,
            ColorComponents.RGBA => StbImageSharp.ColorComponents.RedGreenBlueAlpha,
            _ => throw new System.ArgumentOutOfRangeException(nameof(colorComponents))
        };
    }
}