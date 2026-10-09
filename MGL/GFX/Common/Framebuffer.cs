using MGL.GFX.Shaders;
using MGL.GFX.Textures;
using MGL.Utils;
using Silk.NET.OpenGL;
using VertexArray = MGL.GFX.VertexArray.VertexArray;

namespace MGL.GFX.Common;

public class Framebuffer
{
    private uint _rbo;
    private uint _fbo;
    private uint _texture;
    
    private static VertexArray.VertexArray _quad;
    
    public uint Width { get; private set; }
    public uint Height { get; private set; }
    
    public Texture2D Texture2D { get; private set; }

    private TextureFilter _textureFilter;

    public Framebuffer(uint width, uint height, TextureFilter textureFilter = TextureFilter.Linear)
    {
        Width = width;
        Height = height;
        
        _quad = MeshGenerator.GenQuad();
        _textureFilter = textureFilter;
        
        RecreateBuffers(width, height, textureFilter);
    }
    private unsafe void RecreateBuffers(uint width, uint height, TextureFilter textureFilter)
    {
        if (_fbo != 0)
        {
            Window.Current.GlContext.DeleteFramebuffer(_fbo);
            _fbo = 0;
        }
        if (_texture != 0)
        {
            Window.Current.GlContext.DeleteTexture(_texture);
            _texture = 0;
        }
        if (_rbo != 0)
        {
            Window.Current.GlContext.DeleteRenderbuffer(_rbo);
            _rbo = 0;
        }

        _fbo = Window.Current.GlContext.GenFramebuffer();
        _rbo = Window.Current.GlContext.GenRenderbuffer();
        _texture = Window.Current.GlContext.GenTexture();
        
        Window.Current.GlContext.BindFramebuffer(GLEnum.Framebuffer, _fbo);
        
        Window.Current.GlContext.BindTexture(GLEnum.Texture2D, _texture);
        Window.Current.GlContext.TexImage2D(GLEnum.Texture2D, 0, InternalFormat.Rgb, width, height, 0, PixelFormat.Rgb, GLEnum.UnsignedByte, (void*)0);
        
        if (textureFilter == TextureFilter.Linear)
        {
            Window.Current.GlContext.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            Window.Current.GlContext.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
        }
        else
        {
            Window.Current.GlContext.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
            Window.Current.GlContext.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);
        }
        
        Window.Current.GlContext.FramebufferTexture2D(GLEnum.Framebuffer, FramebufferAttachment.ColorAttachment0, GLEnum.Texture2D, _texture, 0);
        
        Window.Current.GlContext.BindRenderbuffer(GLEnum.Renderbuffer, _rbo);
        Window.Current.GlContext.RenderbufferStorage(GLEnum.Renderbuffer, InternalFormat.Depth24Stencil8, width, height);
        Window.Current.GlContext.FramebufferRenderbuffer(GLEnum.Framebuffer, FramebufferAttachment.DepthStencilAttachment, GLEnum.Renderbuffer, _rbo);
        
        Window.Current.GlContext.BindFramebuffer(GLEnum.Framebuffer, 0);
        Window.Current.GlContext.BindTexture(GLEnum.Texture2D, 0);
        Window.Current.GlContext.BindRenderbuffer(GLEnum.Renderbuffer, 0);
        
        Texture2D = new(_texture);
    }
    
    public void BeginFrame()
    {
        Window.Current.GlContext.BindFramebuffer(GLEnum.Framebuffer, _fbo);
        Window.Current.GlContext.Viewport(0, 0, Width, Height);
    }

    public void EndFrame()
    {
        Window.Current.GlContext.BindFramebuffer(GLEnum.Framebuffer, 0);
    }
    
    public void Resize(uint width, uint height)
    {
        Width = width;
        Height = height;
        TextureFilter currentFilter = _textureFilter;
        
        RecreateBuffers(width, height, currentFilter);
    }

    public void DrawToScreen(uint windowWidth, uint windowHeight)
    {
        RenderCommand.DepthTest = false;
        
        Window.Current.GlContext.Viewport(0, 0, windowWidth, windowHeight);
        
        Texture2D.Bind();
        DefaultShaders.GetBlit().Bind();
        _quad.Draw();
        
        RenderCommand.DepthTest = true;
    }
}