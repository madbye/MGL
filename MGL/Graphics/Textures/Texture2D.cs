using System.Drawing;
using Silk.NET.OpenGL;

namespace MGL.Graphics;

public enum TextureFilter
{
    Linear, 
    Nearest
}

public class Texture2D : IDisposable
{
    public uint Handle { get; private set; }
    public uint Width { get; private set; }
    public uint Height { get; private set; }

    private Renderer _renderer;
    
    public unsafe Texture2D(Renderer renderer, uint width, uint height)
    {
        Width = width;
        Height = height;
        _renderer = renderer;
        
        Handle = _renderer.GlContext.GenTexture();

        Bind();

        _renderer.GlContext.PixelStore(PixelStoreParameter.UnpackAlignment, 1);
        
        _renderer.GlContext.TexImage2D(
            TextureTarget.Texture2D,
            0,
            InternalFormat.Rgba8, 
            width,
            height,
            0,
            PixelFormat.Rgba,
            PixelType.UnsignedByte,
            null                
        );
        
        _renderer.GlContext.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
        _renderer.GlContext.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
        
        _renderer.GlContext.BindTexture(TextureTarget.Texture2D, 0);
    }

    public Texture2D(Renderer renderer, uint handle)
    {
        _renderer = renderer;
        Handle = handle;
        Width = (uint)renderer.GlContext.GetTextureParameter(handle, GetTextureParameter.TextureWidth);
        Height = (uint)renderer.GlContext.GetTextureParameter(handle, GetTextureParameter.TextureHeight);
    }

    public unsafe Texture2D(Renderer renderer, uint width, uint height, byte[] data, ColorComponents colorComponents, TextureFilter textureFilter, bool enableMipmaps)
    {
        uint handle = renderer.GlContext.GenTexture();

        renderer.GlContext.BindTexture(TextureTarget.Texture2D, handle);
        renderer.GlContext.PixelStore(PixelStoreParameter.UnpackAlignment, 1);
        
        switch (colorComponents)
        {
            case ColorComponents.R:
                fixed (byte* ptr = data)
                    renderer.GlContext.TexImage2D(TextureTarget.Texture2D, 0, InternalFormat.Red, width,
                        height, 0, PixelFormat.Red, PixelType.UnsignedByte, ptr);
                break;
            case ColorComponents.RGB:
                fixed (byte* ptr = data)
                    renderer.GlContext.TexImage2D(TextureTarget.Texture2D, 0, InternalFormat.Rgb, width,
                        height, 0, PixelFormat.Rgb, PixelType.UnsignedByte, ptr);
                break;
            case ColorComponents.RGBA:
                fixed (byte* ptr = data)
                    renderer.GlContext.TexImage2D(TextureTarget.Texture2D, 0, InternalFormat.Rgba, width,
                        height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, ptr);
                break;
            default:
                throw new ArgumentException("Unknown image format.");
        }

        
        var minFilter = textureFilter == TextureFilter.Linear 
            ? (enableMipmaps ? TextureMinFilter.LinearMipmapLinear : TextureMinFilter.Linear)
            : (enableMipmaps ? TextureMinFilter.NearestMipmapNearest : TextureMinFilter.Nearest);

        var magFilter = textureFilter == TextureFilter.Linear ? TextureMagFilter.Linear : TextureMagFilter.Nearest;
        
        renderer.GlContext.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
        renderer.GlContext.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);
        renderer.GlContext.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)minFilter);
        renderer.GlContext.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)magFilter);
    
        if(enableMipmaps)
            renderer.GlContext.GenerateMipmap(TextureTarget.Texture2D);
        
        renderer.GlContext.BindTexture(TextureTarget.Texture2D, 0);

        _renderer = renderer;
        Handle = handle;
        Width = width;
        Height = height;
    }

    public static Texture2D FromImage(Renderer renderer, Image image, TextureFilter textureFilter, bool enableMipmaps)
    {
        return new(renderer, image.Width, image.Height, image.PixelData, image.ColorComponents, textureFilter, enableMipmaps);
    }

    public void Bind(byte textureSlot = 0)
    {
        _renderer.GlContext.ActiveTexture(TextureUnit.Texture0 + textureSlot);
        _renderer.GlContext.BindTexture(TextureTarget.Texture2D, Handle);
    }
    
    public unsafe void SetData(Rectangle bounds, byte[] data)
    {
        Bind();
        fixed (byte* ptr = data)
        {
            _renderer.GlContext.TexSubImage2D(
                target: TextureTarget.Texture2D,
                level: 0,
                xoffset: bounds.Left,
                yoffset: bounds.Top,
                width: (uint)bounds.Width,
                height: (uint)bounds.Height,
                format: PixelFormat.Rgba,   
                type: PixelType.UnsignedByte,
                pixels: ptr
            );
        }
    }

    public void Dispose()
    {
        _renderer.GlContext.DeleteTexture(Handle);
    }
}