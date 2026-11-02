using Silk.NET.OpenGL;

namespace MGL.Graphics;

public class TextureArray : IDisposable
{
    public uint Handle { get; private set; }
    public uint Width { get; private set; }
    public uint Height { get; private set; }
    public uint TextureCount { get; private set; }

    private Renderer _renderer;

    public TextureArray(Renderer renderer, Image[] textures, TextureFilter textureFilter = TextureFilter.Nearest, byte mipLevels = 1)
    {
        CheckTextures(textures);
        if (mipLevels < 1) throw new ArgumentException("Mip levels cannot be less than 1.");
        
        _renderer = renderer;
        
        Width = textures[0].Width;
        Height = textures[0].Height;
        

        GLEnum internalFormat = textures[0].ColorComponents switch
        {
            ColorComponents.R => GLEnum.R8,
            ColorComponents.RGB => GLEnum.Rgb8,
            ColorComponents.RGBA => GLEnum.Rgba8,
            _ => GLEnum.Rgba8
        };

        GLEnum pixelFormat = textures[0].ColorComponents switch
        {
            ColorComponents.R => GLEnum.Red,
            ColorComponents.RGB => GLEnum.Rgb,
            ColorComponents.RGBA => GLEnum.Rgba,
            _ => GLEnum.Rgba
        }; 
        
        TextureCount = (uint)textures.Length;
        GL gl = _renderer.GlContext;

        Handle = gl.GenTexture();
        gl.BindTexture(GLEnum.Texture2DArray, Handle);

        gl.TexStorage3D(GLEnum.Texture2DArray, mipLevels, internalFormat, Width, Height, TextureCount);
        
        for (int i = 0; i < textures.Length; i++)
        {
            gl.TexSubImage3D(GLEnum.Texture2DArray, 0, 0, 0, i, Width, Height, 1, pixelFormat, GLEnum.UnsignedByte, textures[i].PixelData);
        }

        if (mipLevels > 1)
        {
            gl.GenerateMipmap(GLEnum.Texture2DArray);
        }
        
        TextureMagFilter magFilter = textureFilter == TextureFilter.Linear ? TextureMagFilter.Linear : TextureMagFilter.Nearest;
        TextureMinFilter minFilter;

        if (mipLevels > 1)
        {
            minFilter = textureFilter == TextureFilter.Linear 
                ? TextureMinFilter.LinearMipmapLinear 
                : TextureMinFilter.NearestMipmapNearest;
        }
        else
        {
            minFilter = textureFilter == TextureFilter.Linear 
                ? TextureMinFilter.Linear 
                : TextureMinFilter.Nearest;
        }

        
        gl.TexParameter(GLEnum.Texture2DArray, GLEnum.TextureWrapS, (int)GLEnum.Repeat);
        gl.TexParameter(GLEnum.Texture2DArray, GLEnum.TextureWrapT, (int)GLEnum.Repeat);
        gl.TexParameter(GLEnum.Texture2DArray, GLEnum.TextureMinFilter, (int)minFilter);
        gl.TexParameter(GLEnum.Texture2DArray, GLEnum.TextureMagFilter, (int)magFilter);
        
        gl.BindTexture(GLEnum.Texture2DArray, 0);
    }

    private void CheckTextures(Image[] textures)
    {
        if (textures == null) throw new ArgumentNullException(nameof(textures), "Texture array cannot be null.");
        if (textures.Length == 0) throw new ArgumentException("Texture array cannot be empty.");
        
        uint width = textures[0].Width;
        uint height = textures[0].Height;
        ColorComponents colorComponents = textures[0].ColorComponents;

        foreach (var texture in textures)
        {
            if (texture == null) throw new ArgumentException("Array contains a null texture element.", nameof(textures));
            if (texture.Width != width || texture.Height != height) throw new ArgumentException("The textures must be the same size.");
            if (texture.ColorComponents != colorComponents) throw new ArgumentException("The textures must be the same pixel format.");
        }
    }

    public void Dispose()
    {
        _renderer.GlContext.DeleteTexture(Handle);
    }
}