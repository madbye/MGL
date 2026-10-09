using System;
using System.Drawing;
using System.Numerics;
using MGL.GFX.Textures;
using Silk.NET.OpenGL;

namespace MGL.GFX.Shaders;

public class ShaderProgram : IDisposable
{
    public uint Handle { get; private set; }
    public static ShaderProgram Current { get; private set; }

    public ShaderProgram(Shader[] shaders)
    {
        Handle = Window.Current.GlContext.CreateProgram();

        foreach (var shader in shaders)
        {
            Window.Current.GlContext.AttachShader(Handle, shader.Handle);
        }
        Window.Current.GlContext.LinkProgram(Handle);
        
        Window.Current.GlContext.GetProgram(Handle, ProgramPropertyARB.LinkStatus, out int lStatus);
        if (lStatus != (int) GLEnum.True)
            throw new Exception($"Program failed to link, status: {lStatus}, error: " + Window.Current.GlContext.GetProgramInfoLog(Handle));

        foreach (var shader in shaders)
        {
            Window.Current.GlContext.DetachShader(Handle, shader.Handle);
        }
    }

    public unsafe void SetUniform(string name, object value)
    {
        
        Bind();
        int location = Window.Current.GlContext.GetUniformLocation(Handle, name);
        switch (value)
        {
            case bool i:
                if (i)
                    Window.Current.GlContext.Uniform1(location, 1);
                else
                    Window.Current.GlContext.Uniform1(location, 0);
                break;
            case int i:
                Window.Current.GlContext.Uniform1(location, i);
                break;
            case float i:
                Window.Current.GlContext.Uniform1(location, i);
                break;
            case Matrix4x4 i:
                Window.Current.GlContext.UniformMatrix4(location, 1, false, (float*)&i);
                break;
            case Vector3 i:
                Window.Current.GlContext.Uniform3(location, i);
                break;
            case Color i:
                Window.Current.GlContext.Uniform3(location, i.R, i.G, i.B);
                break;
            default:
                throw new Exception(value.GetType() + " in not a valid uniform type.");
        }
    }

    public void SetUniformTextureUnit(string name, Texture2D texture2D, int textureUnit)
    {
        Bind();
        int maxUnits = Window.Current.GlContext.GetInteger(GetPName.MaxCombinedTextureImageUnits);
        
        if (maxUnits < textureUnit)
            throw new ArgumentException(textureUnit + " texture unit is not valid, max is " + (maxUnits - 1));
        
        texture2D.Bind((TextureUnit)(33984+textureUnit));
        SetUniform(name, textureUnit);
    }
    public int GetAttribLocation(string name)
    {
        var result = Window.Current.GlContext.GetAttribLocation(Handle, name);
        return result;
    }
    public int GetUniformLocation(string name)
    {
        var result = Window.Current.GlContext.GetUniformLocation(Handle, name);
        return result;
    }
    public void Bind()
    {
        Window.Current.GlContext.UseProgram(Handle);
        Current = this;
    }
    
    public void Dispose()
    {
        Window.Current.GlContext.DeleteProgram(Handle);
    }
}