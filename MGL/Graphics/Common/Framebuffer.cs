using MGL.Utils;
using Silk.NET.OpenGL;

namespace MGL.Graphics;

public class Framebuffer
{
    private uint _rbo;
    private uint _fbo;
    private uint _texture;
    
    private VertexArray _quad;
    
    public uint Width { get; private set; }
    public uint Height { get; private set; }
    
    public Texture2D Texture2D { get; private set; }

    private TextureFilter _textureFilter;

    private Renderer _renderer;
    
    public Framebuffer(Renderer renderer, uint width, uint height, TextureFilter textureFilter = TextureFilter.Linear)
    {
        _renderer = renderer;
        
        Width = width;
        Height = height;
        
        _quad = VertexArray.FromCommonMesh(renderer, MeshGenerator.GenQuad());
        _textureFilter = textureFilter;
        
        RecreateBuffers(width, height, textureFilter);
    }
    private unsafe void RecreateBuffers(uint width, uint height, TextureFilter textureFilter)
    {
        if (_fbo != 0)
        {
            _renderer.GlContext.DeleteFramebuffer(_fbo);
            _fbo = 0;
        }
        if (_texture != 0)
        {
            _renderer.GlContext.DeleteTexture(_texture);
            _texture = 0;
        }
        if (_rbo != 0)
        {
            _renderer.GlContext.DeleteRenderbuffer(_rbo);
            _rbo = 0;
        }

        _fbo = _renderer.GlContext.GenFramebuffer();
        _rbo = _renderer.GlContext.GenRenderbuffer();
        _texture = _renderer.GlContext.GenTexture();
        
        _renderer.GlContext.BindFramebuffer(GLEnum.Framebuffer, _fbo);
        
        _renderer.GlContext.BindTexture(GLEnum.Texture2D, _texture);
        _renderer.GlContext.TexImage2D(GLEnum.Texture2D, 0, InternalFormat.Rgb, width, height, 0, PixelFormat.Rgb, GLEnum.UnsignedByte, (void*)0);
        
        if (textureFilter == TextureFilter.Linear)
        {
            _renderer.GlContext.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            _renderer.GlContext.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
        }
        else
        {
            _renderer.GlContext.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
            _renderer.GlContext.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);
        }
        
        _renderer.GlContext.FramebufferTexture2D(GLEnum.Framebuffer, FramebufferAttachment.ColorAttachment0, GLEnum.Texture2D, _texture, 0);
        
        _renderer.GlContext.BindRenderbuffer(GLEnum.Renderbuffer, _rbo);
        _renderer.GlContext.RenderbufferStorage(GLEnum.Renderbuffer, InternalFormat.Depth24Stencil8, width, height);
        _renderer.GlContext.FramebufferRenderbuffer(GLEnum.Framebuffer, FramebufferAttachment.DepthStencilAttachment, GLEnum.Renderbuffer, _rbo);
        
        _renderer.GlContext.BindFramebuffer(GLEnum.Framebuffer, 0);
        _renderer.GlContext.BindTexture(GLEnum.Texture2D, 0);
        _renderer.GlContext.BindRenderbuffer(GLEnum.Renderbuffer, 0);
        
        Texture2D = new(_renderer, _texture);
    }
    
    public void BeginFrame()
    {
        _renderer.GlContext.BindFramebuffer(GLEnum.Framebuffer, _fbo);
        _renderer.GlContext.Viewport(0, 0, Width, Height);
    }

    public void EndFrame()
    {
        _renderer.GlContext.BindFramebuffer(GLEnum.Framebuffer, 0);
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
        _renderer.Disable(EnableCap.DepthTest);
        
        _renderer.GlContext.Viewport(0, 0, windowWidth, windowHeight);
        
        Texture2D.Bind();
        _renderer.DefaultShaders.GetBlit().Bind();
        _quad.Draw();
        
        _renderer.Enable(EnableCap.DepthTest);
    }
}