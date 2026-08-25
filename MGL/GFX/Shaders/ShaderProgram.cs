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
        Handle = Env.Gl.CreateProgram();

        foreach (var shader in shaders)
        {
            Env.Gl.AttachShader(Handle, shader.Handle);
        }
        Env.Gl.LinkProgram(Handle);
        
        Env.Gl.GetProgram(Handle, ProgramPropertyARB.LinkStatus, out int lStatus);
        if (lStatus != (int) GLEnum.True)
            throw new Exception($"Program failed to link, status: {lStatus}, error: " + Env.Gl.GetProgramInfoLog(Handle));

        foreach (var shader in shaders)
        {
            Env.Gl.DetachShader(Handle, shader.Handle);
        }
    }

    public unsafe void SetUniform(string name, object value)
    {
        Bind();
        int location = Env.Gl.GetUniformLocation(Handle, name);
        switch (value)
        {
            case bool i:
                if (i)
                    Env.Gl.Uniform1(location, 1);
                else
                    Env.Gl.Uniform1(location, 0);
                break;
            case int i:
                Env.Gl.Uniform1(location, i);
                break;
            case float i:
                Env.Gl.Uniform1(location, i);
                break;
            case Matrix4x4 i:
                Env.Gl.UniformMatrix4(location, 1, false, (float*)&i);
                break;
            case Vector3 i:
                Env.Gl.Uniform3(location, i);
                break;
            case Color i:
                Env.Gl.Uniform3(location, i.R, i.G, i.B);
                break;
            default:
                throw new Exception(value.GetType() + " in not a valid uniform type.");
                break;
        }
    }

    public void SetUniformTextureUnit(string name, Texture2D texture2D, byte textureUnit)
    {
        texture2D.Bind((TextureUnit)(33984+textureUnit));
        ShaderProgram.Current.SetUniform(name, (int)textureUnit);
    }
    public int GetAttribLocation(string name)
    {
        var result = Env.Gl.GetAttribLocation(Handle, name);
        return result;
    }
    public int GetUniformLocation(string name)
    {
        var result = Env.Gl.GetUniformLocation(Handle, name);
        return result;
    }
    public void Bind()
    {
        Current = this;
        Env.Gl.UseProgram(Handle);
    }
    
    public void Dispose()
    {
        Env.Gl.DeleteProgram(Handle);
    }
}