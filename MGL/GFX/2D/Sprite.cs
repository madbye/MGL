using System.Numerics;
using MGL.GFX.Common;
using MGL.GFX.Shaders;
using MGL.GFX.Textures;
using MGL.Utils;

namespace MGL.GFX._2D;

public class Sprite
{
    private static VertexArray.VertexArray? _quad;
    private Texture2D _texture;
    
    public Sprite(Image image, TextureFilter texFilter)
    {
        _quad ??= MeshGenerator.GenQuad((float)image.Width / image.Height);
        _texture = Texture2D.FromImage(image, texFilter, true);
    }

    public void Draw(ICamera camera, Matrix4x4 matrix, ShaderProgram? program = null)
    {
        ShaderProgram? shaderProgram = program;
        shaderProgram ??= DefaultShaders.GetUnlit();
        
        shaderProgram.Bind();
        shaderProgram.SetUniform("view", camera.GetViewMatrix());
        shaderProgram.SetUniform("proj", camera.GetProjectionMatrix());
        shaderProgram.SetUniform("model", matrix);
        
        _texture.Bind();
        
        _quad?.Draw();
    }
}