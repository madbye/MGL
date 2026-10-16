using Silk.NET.OpenGL;

namespace MGL.Graphics;

public enum ShaderType
{
    Vertex,
    Fragment
}

public class Shader(Renderer renderer, string source, ShaderType shaderType) : IDisposable
{
    public uint Handle { get; private set; }

    public ShaderType Type { get; } = shaderType;
    public string Source { get; } = source;

    private Renderer _renderer = renderer;

    public void Compile()
    {
        switch (Type)
        {
            case ShaderType.Vertex:
                Handle = _renderer.GlContext.CreateShader(GLEnum.VertexShader);
                break;
            case ShaderType.Fragment:
                Handle = _renderer.GlContext.CreateShader(GLEnum.FragmentShader);
                break;
        }
        _renderer.GlContext.ShaderSource(Handle, Source);
        
        _renderer.GlContext.CompileShader(Handle);
        _renderer.GlContext.GetShader(Handle, ShaderParameterName.CompileStatus, out int status);
        if (status != (int) GLEnum.True)
            throw new Exception(Type + " shader failed to compile: " + _renderer.GlContext.GetShaderInfoLog(Handle));
    }

    public void Dispose()
    {
        _renderer.GlContext.DeleteShader(Handle);
    }
}