using System.Numerics;
using MGL.GFX.Shaders;
using MGL.GFX.Textures;
using MGL.GFX.VertexAttributes;
using MGL.IO;
using MGL.Utils;

namespace MGL.GFX.Common;

public class Skybox
{
    private Texture2D _texture2D;
    private VAO _vao;
    private ShaderProgram? _shaderProgram;

    public Skybox(string path, TextureFilter textureFilter, ShaderProgram? program = null)
    {
        _texture2D = Texture2D.FromImage(ImageLoader.LoadImage(path, ColorComponents.RGB), textureFilter, false);
        _vao = MeshGenerator.GenUVSphere(64, 32);

        _shaderProgram = program;
        _shaderProgram ??= DefaultShaders.GetUnlit();
    }

    public void DrawSkybox(PerspectiveCamera perspectiveCamera)
    {
        _shaderProgram.Bind();
        _shaderProgram.SetUniform("view", perspectiveCamera.GetViewMatrix());
        _shaderProgram.SetUniform("proj", perspectiveCamera.GetProjectionMatrix());
        _shaderProgram.SetUniform("model", Matrix4x4.CreateRotationX(1.5708f) * Matrix4x4.CreateTranslation(perspectiveCamera.Position));
        
        _texture2D.Bind();

        RenderCommand.DepthTest = false;
        _vao.Draw();
        RenderCommand.DepthTest = true;
    }
}