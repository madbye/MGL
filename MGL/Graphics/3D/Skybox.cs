using System.Numerics;
using MGL.IO;
using MGL.Utils;

namespace MGL.Graphics;

public class Skybox
{
    private Texture2D _texture2D;
    private VertexArray _vertexArray;
    private Renderer _renderer;

    public Skybox(Renderer renderer, Image image, TextureFilter textureFilter = TextureFilter.Linear, uint stackCount = 64, uint sectorCount = 32)
    {
        _renderer = renderer;
        _texture2D = Texture2D.FromImage(_renderer, image, textureFilter, false);
        _vertexArray = VertexArray.FromCommonMesh(_renderer, MeshGenerator.GenUVSphere(stackCount, sectorCount)); 
    }

    public void Draw(ICamera camera, ShaderProgram? program = null)
    {
        ShaderProgram? shaderProgram = program;
        shaderProgram ??= _renderer.DefaultShaders.GetUnlit();
        
        shaderProgram.Bind();
        shaderProgram.SetUniform("view", camera.GetViewMatrix());
        shaderProgram.SetUniform("proj", camera.GetProjectionMatrix());
        shaderProgram.SetUniform("model", Matrix4x4.CreateRotationX(180 * MathF.PI / 180) *  Matrix4x4.CreateTranslation(camera.Position));
        
        _texture2D.Bind();

        _renderer.Disable(EnableCap.DepthTest);
        _vertexArray.Draw();
        _renderer.Enable(EnableCap.DepthTest);
    }
}