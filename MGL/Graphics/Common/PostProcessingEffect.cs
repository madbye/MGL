using System.Reflection;
using MGL.Utils;

namespace MGL.Graphics;

public class PostProcessingEffect
{
    public ShaderProgram Shader { get; private set; }
    public Framebuffer Framebuffer { get; private set; }

    private Renderer _renderer;
    private VertexArray _quad;
    
    public PostProcessingEffect(Renderer renderer, Shader shader, TextureFilter textureFilter = TextureFilter.Linear)
    {
        _renderer = renderer;
        if (shader.Type != ShaderType.Fragment)
            throw new ArgumentException("Shader must be fragment");
        
        using (var reader = new StreamReader(Assembly.GetExecutingAssembly().GetManifestResourceStream(Path.Combine("MGL.Resources.Shaders.Blit.shader.vert"))))
        {
            var vert = new Shader(renderer, reader.ReadToEnd(), ShaderType.Vertex);
            vert.Compile();
            Shader = new(renderer, new []{vert, shader});
        }

        Framebuffer = new Framebuffer( renderer, 1, 1, textureFilter);

        _quad = VertexArray.FromCommonMesh(renderer, MeshGenerator.GenQuad());
    }

    private uint width, height;
    public void Apply(Texture2D texture2D)
    {
        if (texture2D.Width != width || texture2D.Height != height)
        {
            width = texture2D.Width;
            height = texture2D.Height;
            Framebuffer.Resize(texture2D.Width, texture2D.Height);
        }
        
        _renderer.Disable(EnableCap.DepthTest);
        Framebuffer.BeginFrame();
        
        Shader.Bind();
        texture2D.Bind();
        _quad.Draw();
        
        Framebuffer.EndFrame();
        _renderer.Enable(EnableCap.DepthTest);
    }
    
}