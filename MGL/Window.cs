using MGL.Input;
using Silk.NET.Input;
using Silk.NET.Maths;
using Silk.NET.OpenGL;
using Silk.NET.Windowing;

namespace MGL;

public class Window
{
    private IWindow _windowHandle;
    private GL _gl;
    
    public event Action<uint, uint>? OnResize;

    public Window(uint width, uint height, string title)
    {
        var options = WindowOptions.Default with
        {
            Size = new Vector2D<int>((int)width, (int)height),
            Title = title,
            IsEventDriven = false
        };
        _windowHandle = Silk.NET.Windowing.Window.Create(options);
        _windowHandle.Resize += (d =>
        {
            OnResize?.Invoke((uint)d.X, (uint)d.Y);
            Env.Gl.Viewport(0, 0, (uint)d.X, (uint)d.Y);
        });
           
    }
    
    public void Initialize()
    { 
        _windowHandle.Initialize();
        _gl = _windowHandle.CreateOpenGL();
        Bind();
    }

    public InputContext GetInputContext() => new(_windowHandle.CreateInput());
    public void SwapBuffers() => _windowHandle.SwapBuffers();
    public void DoEvents() => _windowHandle.DoEvents();
    public bool ShouldClose() => _windowHandle.IsClosing;
    public void Close() => _windowHandle.Close();
    public void Bind() => Env.Gl = _gl;
    
    public (uint, uint) GetSize()
    {
        return new((uint)_windowHandle.Size.X, (uint)_windowHandle.Size.Y);
    }
}