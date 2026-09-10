using System.Numerics;
using MGL.GFX.Common;
using MGL.GFX.Shaders;
using MGL.GFX.Textures;
using MGL.IO;
using MGL.Utils;

namespace MGL.GFX._3D;

public class Skybox
{
    private Texture2D _texture2D;
    private VertexArray.VertexArray _vertexArray;

    public Skybox(string path, TextureFilter textureFilter)
    {
        _texture2D = Texture2D.FromImage(ImageLoader.LoadImage(path, ColorComponents.RGB), textureFilter, false);
        _vertexArray = MeshGenerator.GenUVSphere(64, 32);
    }

    public void DrawSkybox(ICamera camera, ShaderProgram? program = null)
    {
        ShaderProgram? shaderProgram = program;
        shaderProgram ??= DefaultShaders.GetUnlit();
        
        shaderProgram.Bind();
        shaderProgram.SetUniform("view", camera.GetViewMatrix());
        shaderProgram.SetUniform("proj", camera.GetProjectionMatrix());
        shaderProgram.SetUniform("model", Matrix4x4.CreateRotationX(1.5708f) * Matrix4x4.CreateTranslation(camera.Position));
        
        _texture2D.Bind();

        RenderCommand.DepthTest = false;
        _vertexArray.Draw();
        RenderCommand.DepthTest = true;
    }
}