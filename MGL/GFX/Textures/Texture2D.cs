using System.Drawing;
using MGL.GFX.Common;
using Silk.NET.OpenGL;

namespace MGL.GFX.Textures;

public enum TextureFilter
{
    Linear, 
    Nearest
}

public class Texture2D
{
    public uint Handle { get; private set; }
    public uint Width { get; private set; }
    public uint Height { get; private set; }
    
    public unsafe Texture2D(uint width, uint height)
    {
        Width = width;
        Height = height;

        Handle = Window.Current.GlContext.GenTexture();

        Bind();

        Window.Current.GlContext.PixelStore(PixelStoreParameter.UnpackAlignment, 1);
        
        Window.Current.GlContext.TexImage2D(
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
        
        Window.Current.GlContext.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
        Window.Current.GlContext.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
        
        Window.Current.GlContext.BindTexture(TextureTarget.Texture2D, 0);
    }

    public Texture2D(uint handle)
    {
        Handle = handle;
        Width = (uint)Window.Current.GlContext.GetTextureParameter(handle, GetTextureParameter.TextureWidth);
        Height = (uint)Window.Current.GlContext.GetTextureParameter(handle, GetTextureParameter.TextureHeight);
    }

    public static unsafe Texture2D FromImage(Image image, TextureFilter textureFilter, bool enableMipmaps)
    {
        uint handle = Window.Current.GlContext.GenTexture();

        Window.Current.GlContext.BindTexture(TextureTarget.Texture2D, handle);

        Window.Current.GlContext.PixelStore(PixelStoreParameter.UnpackAlignment, 1);
        
        switch (image.Channels)
        {
            case 1:
                fixed (byte* ptr = image.PixelData)
                    Window.Current.GlContext.TexImage2D(TextureTarget.Texture2D, 0, InternalFormat.Red, image.Width,
                        image.Height, 0, PixelFormat.Red, PixelType.UnsignedByte, ptr);
                break;
            case 3:
                fixed (byte* ptr = image.PixelData)
                    Window.Current.GlContext.TexImage2D(TextureTarget.Texture2D, 0, InternalFormat.Rgb, image.Width,
                        image.Height, 0, PixelFormat.Rgb, PixelType.UnsignedByte, ptr);
                break;
            case 4:
                fixed (byte* ptr = image.PixelData)
                    Window.Current.GlContext.TexImage2D(TextureTarget.Texture2D, 0, InternalFormat.Rgba, image.Width,
                        image.Height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, ptr);
                break;
        }

        
        var minFilter = textureFilter == TextureFilter.Linear 
            ? (enableMipmaps ? TextureMinFilter.LinearMipmapLinear : TextureMinFilter.Linear)
            : (enableMipmaps ? TextureMinFilter.NearestMipmapNearest : TextureMinFilter.Nearest);

        var magFilter = textureFilter == TextureFilter.Linear ? TextureMagFilter.Linear : TextureMagFilter.Nearest;
        
        Window.Current.GlContext.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
        Window.Current.GlContext.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);
        Window.Current.GlContext.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)minFilter);
        Window.Current.GlContext.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)magFilter);
    
        if(enableMipmaps)
            Window.Current.GlContext.GenerateMipmap(TextureTarget.Texture2D);
        
        Window.Current.GlContext.BindTexture(TextureTarget.Texture2D, 0);

        return new(handle);
    }

    public void Bind(TextureUnit textureSlot = TextureUnit.Texture0)
    {
        Window.Current.GlContext.ActiveTexture(textureSlot);
        Window.Current.GlContext.BindTexture(TextureTarget.Texture2D, Handle);
    }
    
    public unsafe void SetData(Rectangle bounds, byte[] data)
    {
        Bind();
        fixed (byte* ptr = data)
        {
            Window.Current.GlContext.TexSubImage2D(
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
}