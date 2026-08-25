using MGL.GFX.Shaders;
using MGL.GFX.Textures;
using MGL.GFX.VertexAttributes;
using MGL.Utils;
using Silk.NET.OpenGL;

namespace MGL.GFX.Common;

public class Framebuffer
{
    private uint _rbo;
    private uint _fbo;
    private uint _texture;
    
    private static VAO _quad;
    
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
            Env.Gl.DeleteFramebuffer(_fbo);
            _fbo = 0;
        }
        if (_texture != 0)
        {
            Env.Gl.DeleteTexture(_texture);
            _texture = 0;
        }
        if (_rbo != 0)
        {
            Env.Gl.DeleteRenderbuffer(_rbo);
            _rbo = 0;
        }

        _fbo = Env.Gl.GenFramebuffer();
        _rbo = Env.Gl.GenRenderbuffer();
        _texture = Env.Gl.GenTexture();
        
        Env.Gl.BindFramebuffer(GLEnum.Framebuffer, _fbo);
        
        Env.Gl.BindTexture(GLEnum.Texture2D, _texture);
        Env.Gl.TexImage2D(GLEnum.Texture2D, 0, InternalFormat.Rgb, width, height, 0, PixelFormat.Rgb, GLEnum.UnsignedByte, (void*)0);
        
        if (textureFilter == TextureFilter.Linear)
        {
            Env.Gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            Env.Gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
        }
        else
        {
            Env.Gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
            Env.Gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);
        }
        
        Env.Gl.FramebufferTexture2D(GLEnum.Framebuffer, FramebufferAttachment.ColorAttachment0, GLEnum.Texture2D, _texture, 0);
        
        Env.Gl.BindRenderbuffer(GLEnum.Renderbuffer, _rbo);
        Env.Gl.RenderbufferStorage(GLEnum.Renderbuffer, InternalFormat.Depth24Stencil8, width, height);
        Env.Gl.FramebufferRenderbuffer(GLEnum.Framebuffer, FramebufferAttachment.DepthStencilAttachment, GLEnum.Renderbuffer, _rbo);
        
        Env.Gl.BindFramebuffer(GLEnum.Framebuffer, 0);
        Env.Gl.BindTexture(GLEnum.Texture2D, 0);
        Env.Gl.BindRenderbuffer(GLEnum.Renderbuffer, 0);
        
        Texture2D = new(_texture, width, height);
    }
    
    public void BeginFrame()
    {
        Env.Gl.BindFramebuffer(GLEnum.Framebuffer, _fbo);
        Env.Gl.Viewport(0, 0, Width, Height);
    }

    public void EndFrame()
    {
        Env.Gl.BindFramebuffer(GLEnum.Framebuffer, 0);
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
        
        Env.Gl.Viewport(0, 0, windowWidth, windowHeight);
        
        Texture2D.Bind();
        DefaultShaders.GetBlit().Bind();
        _quad.Draw();
        
        RenderCommand.DepthTest = true;
    }
}