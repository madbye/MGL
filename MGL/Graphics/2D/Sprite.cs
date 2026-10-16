using System.Numerics;
using MGL.Utils;

namespace MGL.Graphics;

public class Sprite
{
    private static VertexArray? _quad;
    private Texture2D _texture;
    private Renderer _renderer;
    
    public Sprite(Renderer renderer, Image image, TextureFilter texFilter)
    {
        _renderer = renderer;
        _quad ??= VertexArray.FromCommonMesh(renderer, MeshGenerator.GenQuad((float)image.Width / image.Height)); 
        _texture = Texture2D.FromImage(renderer, image, texFilter, true);
    }

    public void Draw(ICamera camera, Matrix4x4 matrix, ShaderProgram? program = null)
    {
        ShaderProgram? shaderProgram = program;
        shaderProgram ??= _renderer.DefaultShaders.GetUnlit();
        
        shaderProgram.Bind();
        shaderProgram.SetUniform("view", camera.GetViewMatrix());
        shaderProgram.SetUniform("proj", camera.GetProjectionMatrix());
        shaderProgram.SetUniform("model", matrix);
        
        _texture.Bind();
        
        _quad?.Draw();
    }
}