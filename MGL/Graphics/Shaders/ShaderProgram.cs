using System.Drawing;
using System.Numerics;
using Silk.NET.OpenGL;

namespace MGL.Graphics;

public class ShaderProgram : IDisposable
{
    public uint Handle { get; private set; }
    public static ShaderProgram Current { get; private set; }

    private Renderer _renderer;
    
    public ShaderProgram(Renderer renderer, Shader[] shaders)
    {
        _renderer = renderer;
        
        Handle = _renderer.GlContext.CreateProgram();

        foreach (var shader in shaders)
        {
            _renderer.GlContext.AttachShader(Handle, shader.Handle);
        }
        _renderer.GlContext.LinkProgram(Handle);
        
        _renderer.GlContext.GetProgram(Handle, ProgramPropertyARB.LinkStatus, out int lStatus);
        if (lStatus != (int) GLEnum.True)
            throw new Exception($"Program failed to link, status: {lStatus}, error: " + _renderer.GlContext.GetProgramInfoLog(Handle));

        foreach (var shader in shaders)
        {
            _renderer.GlContext.DetachShader(Handle, shader.Handle);
            shader.Dispose();
        }
    }

    public unsafe void SetUniform(string name, object value)
    {
        
        Bind();
        int location = _renderer.GlContext.GetUniformLocation(Handle, name);
        switch (value)
        {
            case bool i:
                if (i)
                    _renderer.GlContext.Uniform1(location, 1);
                else
                    _renderer.GlContext.Uniform1(location, 0);
                break;
            case int i:
                _renderer.GlContext.Uniform1(location, i);
                break;
            case float i:
                _renderer.GlContext.Uniform1(location, i);
                break;
            case byte i:
                _renderer.GlContext.Uniform1(location, i);
                break;
            case Matrix4x4 i:
                _renderer.GlContext.UniformMatrix4(location, 1, false, (float*)&i);
                break;
            case Vector3 i:
                _renderer.GlContext.Uniform3(location, i);
                break;
            case Color i:
                _renderer.GlContext.Uniform3(location, i.R, i.G, i.B);
                break;
            default:
                throw new Exception(value.GetType() + " in not a valid uniform type.");
        }
    }

    public void SetUniformTextureUnit(string name, Texture2D texture2D, byte textureUnit)
    {
        Bind();
        int maxUnits = _renderer.GlContext.GetInteger(GetPName.MaxCombinedTextureImageUnits);
        
        if (maxUnits < textureUnit)
            throw new ArgumentException(textureUnit + " texture unit is not valid, max is " + (maxUnits - 1));
        
        texture2D.Bind(textureUnit);
        SetUniform(name, textureUnit);
    }
    public int GetAttribLocation(string name)
    {
        var result = _renderer.GlContext.GetAttribLocation(Handle, name);
        return result;
    }
    public int GetUniformLocation(string name)
    {
        var result = _renderer.GlContext.GetUniformLocation(Handle, name);
        return result;
    }
    public void Bind()
    {
        _renderer.GlContext.UseProgram(Handle);
        Current = this;
    }
    
    public void Dispose()
    {
        _renderer.GlContext.DeleteProgram(Handle);
    }
}