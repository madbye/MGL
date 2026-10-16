using System.Reflection;
using MGL.Utils;

namespace MGL.Graphics;

public class PostProcessingEffect
{
    public ShaderProgram _program;
    private VertexArray _vertexArray;
    public Framebuffer Framebuffer { get; private set; }

    private Renderer _renderer;
    
    public PostProcessingEffect(Renderer renderer, Shader shader, TextureFilter textureFilter = TextureFilter.Linear)
    {
        _renderer = renderer;
        if (shader.Type != ShaderType.Fragment)
            throw new ArgumentException("Shader must be fragment");
        
        using (var reader = new StreamReader(Assembly.GetExecutingAssembly().GetManifestResourceStream(Path.Combine("MGL.Resources.Shaders.Blit.shader.vert"))))
        {
            var vert = new Shader(renderer, reader.ReadToEnd(), ShaderType.Vertex);
            vert.Compile();
            _program = new(renderer, new []{vert, shader});
        }

        Framebuffer = new Framebuffer( renderer, 1, 1, textureFilter);

        _vertexArray = VertexArray.FromCommonMesh(renderer, MeshGenerator.GenQuad());
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
        
        _program.Bind();
        texture2D.Bind();
        _vertexArray.Draw();
        
        Framebuffer.EndFrame();
        _renderer.Enable(EnableCap.DepthTest);
    }
    
}