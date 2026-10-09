using System.Drawing;
using Silk.NET.OpenGL;

namespace MGL.GFX.Common;

public static class RenderCommand
{
    private static bool _depthTest = true;
    
    public static bool DepthTest
    {
        get => _depthTest;
        set
        {
            _depthTest = value;
            if (value)
                Window.Current.GlContext.Enable(EnableCap.DepthTest);
            else
                Window.Current.GlContext.Disable(EnableCap.DepthTest);
        }
    }
    
    private static bool _blend = true;
    
    public static bool Blend
    {
        get => _blend;
        set
        {
            _blend = value;
            if (value)
                Window.Current.GlContext.Enable(EnableCap.Blend);
            else
                Window.Current.GlContext.Disable(EnableCap.Blend);
        }
    }
    
    private static bool _multisample = true;
    
    public static bool Multisample
    {
        get => _multisample;
        set
        {
            _multisample = value;
            if (value)
                Window.Current.GlContext.Enable(EnableCap.Multisample);
            else
                Window.Current.GlContext.Disable(EnableCap.Multisample);
        }
    }
    public static void ClearColor(Color color)
    {
        Window.Current.GlContext.ClearColor(color);
        Window.Current.GlContext.Clear(ClearBufferMask.ColorBufferBit);
    }
    public static void ClearDepth()
    {
        Window.Current.GlContext.Clear(ClearBufferMask.DepthBufferBit);
    }
}