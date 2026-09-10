using System;
using System.IO;
using MGL.GFX.Shaders;
using MGL.GFX.Textures;
using MGL.Utils;

namespace MGL.GFX.Common;

public class PostProcessingEffect
{
    public ShaderProgram _program;
    private VertexArray.VertexArray _vertexArray;
    public Framebuffer Framebuffer { get; private set; }
    
    public PostProcessingEffect( Shader shader, TextureFilter textureFilter = TextureFilter.Linear)
    {
        if (shader.Type != ShaderType.Fragment)
            throw new ArgumentException("Shader must be fragment");
        
        string path = Path.Combine("data", "shaders", "quad.vert");
        Shader vertShader = new Shader(File.ReadAllText(path), ShaderType.Vertex);
        vertShader.Compile();
        _program = new(new []{vertShader, shader});

        Framebuffer = new Framebuffer( 1280, 720, textureFilter);

        _vertexArray = MeshGenerator.GenQuad();
    }

    private uint width, height;
    public void Apply(Texture2D texture2D)
    {
        if (texture2D.Width != width || texture2D.Height != height)
        {
            width = (uint)texture2D.Width;
            height = (uint)texture2D.Height;
            Framebuffer.Resize((uint)texture2D.Width, (uint)texture2D.Height);
        }
        
        RenderCommand.DepthTest = false;
        Framebuffer.BeginFrame();
        
        _program.Bind();
        texture2D.Bind();
        _vertexArray.Draw();
        
        Framebuffer.EndFrame();
        RenderCommand.DepthTest = true;
    }
    
}