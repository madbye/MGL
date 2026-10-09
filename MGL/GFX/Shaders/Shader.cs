using System;
using Silk.NET.OpenGL;

namespace MGL.GFX.Shaders;

public enum ShaderType
{
    Vertex,
    Fragment,
    Geometry
}

public class Shader(string source, ShaderType shaderType) : IDisposable
{
    public uint Handle { get; private set; }

    public ShaderType Type { get; } = shaderType;
    public string Source { get; } = source;

    public void Compile()
    {
        switch (Type)
        {
            case ShaderType.Vertex:
                Handle = Window.Current.GlContext.CreateShader(GLEnum.VertexShader);
                break;
            case ShaderType.Fragment:
                Handle = Window.Current.GlContext.CreateShader(GLEnum.FragmentShader);
                break;
        }
        Window.Current.GlContext.ShaderSource(Handle, Source);
        
        Window.Current.GlContext.CompileShader(Handle);
        Window.Current.GlContext.GetShader(Handle, ShaderParameterName.CompileStatus, out int status);
        if (status != (int) GLEnum.True)
            throw new Exception(Type + " shader failed to compile: " + Window.Current.GlContext.GetShaderInfoLog(Handle));
    }

    public void Dispose()
    {
        Window.Current.GlContext.DeleteShader(Handle);
    }
}