namespace MGL.GFX.Shaders;

public static class DefaultShaders
{
    public static ShaderProgram GetPhong()
    {
        return Window.Current.WindowShaders.GetPhong();
    }

    public static ShaderProgram GetUnlit()
    {
        return Window.Current.WindowShaders.GetUnlit();
    }

    public static ShaderProgram GetText()
    {
        return Window.Current.WindowShaders.GetText();
    }

    public static ShaderProgram GetBlit()
    {
        return Window.Current.WindowShaders.GetBlit();
    }

    public static ShaderProgram GetPBR()
    {
        return Window.Current.WindowShaders.GetPBR();
    }
}