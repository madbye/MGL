using System.Drawing;
using Silk.NET.Maths;
using Silk.NET.OpenGL;

namespace MGL.Graphics;

public class Renderer
{
    internal GL GlContext;
    public DefaultShaders DefaultShaders;

    internal Renderer(GL gl)
    {
        GlContext = gl;
        DefaultShaders = new(this);
    }
    
    public void Enable(EnableCap enableCap)
    {
        GlContext.Enable((Silk.NET.OpenGL.EnableCap)enableCap);
    }

    public void Disable(EnableCap enableCap)
    {
        GlContext.Disable((Silk.NET.OpenGL.EnableCap)enableCap);
    }

    public bool IsEnabled(EnableCap enableCap)
    {
        return GlContext.IsEnabled((Silk.NET.OpenGL.EnableCap)enableCap);
    }
    
    public void ClearColor(Color color)
    {
        GlContext.ClearColor(color);
        GlContext.Clear(ClearBufferMask.ColorBufferBit);
    }
    
    public void ClearDepth()
    {
        GlContext.Clear(ClearBufferMask.DepthBufferBit);
    }

    public void Viewport(int x, int y)
    {
        GlContext.Viewport(new Vector2D<int>(x, y));
    }
}