using System.Drawing;
using System.Numerics;
using Silk.NET.OpenGL;

namespace MGL.Graphics;

public class ShaderProgram : IDisposable
{
    public uint Handle { get; private set; }
    public static ShaderProgram? Current { get; private set; }

    private Dictionary<string, int> _cachedUniformLocations = new Dictionary<string, int>();
    private Dictionary<string, int> _cachedAttributeLocations = new Dictionary<string, int>();
    private readonly Renderer _renderer;
    
    public ShaderProgram(Renderer renderer, Shader[] shaders)
    {
        _renderer = renderer;
        
        Handle = _renderer.GlContext.CreateProgram();

        foreach (var shader in shaders) _renderer.GlContext.AttachShader(Handle, shader.Handle);
        
        _renderer.GlContext.LinkProgram(Handle);
        _renderer.GlContext.GetProgram(Handle, ProgramPropertyARB.LinkStatus, out int lStatus);
        
        if (lStatus != (int) GLEnum.True) throw new Exception($"Program failed to link, status: {lStatus}, error: " + _renderer.GlContext.GetProgramInfoLog(Handle));
        
        foreach (var shader in shaders) _renderer.GlContext.DetachShader(Handle, shader.Handle);
    }

    public void SetUniform(string name, bool value)
    {
        int location = GetUniformLocation(name);
        _renderer.GlContext.Uniform1(location, value ? 1 : 0);
    }
    public void SetUniform(string name, int value) => _renderer.GlContext.Uniform1(GetUniformLocation(name), value);
    public void SetUniform(string name, float value) => _renderer.GlContext.Uniform1(GetUniformLocation(name), value);
    public unsafe void SetUniform(string name, Matrix4x4 value) => _renderer.GlContext.UniformMatrix4(GetUniformLocation(name), 1, false, (float*)&value);
    public void SetUniform(string name, Vector3 value) => _renderer.GlContext.Uniform3(GetUniformLocation(name), value);
    public void SetUniform(string name, Color value) => _renderer.GlContext.Uniform3(GetUniformLocation(name), value.R / 255.0f, value.G / 255.0f, value.B / 255.0f);
    
    
    public void SetUniformTextureUnit(string name, Texture2D texture2D, byte textureUnit)
    {
        Bind();
        int maxUnits = _renderer.GlContext.GetInteger(GetPName.MaxCombinedTextureImageUnits);
        
        if (textureUnit >= maxUnits) throw new ArgumentException(textureUnit + " texture unit is not valid, max is " + (maxUnits - 1));
        
        texture2D.Bind(textureUnit);
        SetUniform(name, textureUnit);
    }
    public int GetAttribLocation(string name)
    {
        if (_cachedAttributeLocations.TryGetValue(name, out var location)) return location;
        
        var result = _renderer.GlContext.GetAttribLocation(Handle, name);
        _cachedAttributeLocations.Add(name, result);
        
        return result;
    }
    public int GetUniformLocation(string name)
    {
        if (_cachedUniformLocations.TryGetValue(name, out var location)) return location;
        
        var result = _renderer.GlContext.GetUniformLocation(Handle, name);
        _cachedUniformLocations.Add(name, result);
        
        return result;
    }
    public void Bind()
    {
        _renderer.GlContext.UseProgram(Handle);
        Current = this;
    }
    
    public void Dispose() => _renderer.GlContext.DeleteProgram(Handle);
}