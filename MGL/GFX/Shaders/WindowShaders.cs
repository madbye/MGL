using System.Reflection;

namespace MGL.GFX.Shaders;

internal class WindowShaders
{
    private ShaderProgram? _phong;
    private ShaderProgram? _unlit;
    private ShaderProgram? _text;
    private ShaderProgram? _blit;
    private ShaderProgram? _pbr;

    public ShaderProgram GetPhong()
    {
        _phong ??= LoadProgram("MGL.Resources.Shaders.Phong.shader.frag",
            "MGL.Resources.Shaders.Phong.shader.vert");

        return _phong;
    }

    public ShaderProgram GetUnlit()
    {
        _unlit ??= LoadProgram("MGL.Resources.Shaders.Unlit.shader.frag",
            "MGL.Resources.Shaders.Unlit.shader.vert");

        return _unlit;
    }
    public ShaderProgram GetText()
    {
        _text ??= LoadProgram("MGL.Resources.Shaders.Text.shader.frag",
            "MGL.Resources.Shaders.Text.shader.vert");
        
        return _text;
    }
    public ShaderProgram GetBlit()
    {
        _blit ??= LoadProgram("MGL.Resources.Shaders.Blit.shader.frag",
            "MGL.Resources.Shaders.Blit.shader.vert");
        
        return _blit;
    }
    public ShaderProgram GetPBR()
    {
        _pbr ??= LoadProgram("MGL.Resources.Shaders.PBR.shader.frag",
            "MGL.Resources.Shaders.PBR.shader.vert");
        
        return _pbr;
    }
    private ShaderProgram LoadProgram(string fragment, string vertex)
    {
        Shader frag;
        Shader vert;
        using (var reader = new StreamReader(Assembly.GetExecutingAssembly().GetManifestResourceStream(fragment)))
        {
            frag = new Shader(reader.ReadToEnd(), ShaderType.Fragment);
        }
        using (var reader = new StreamReader(Assembly.GetExecutingAssembly().GetManifestResourceStream(vertex)))
        {
            vert = new Shader(reader.ReadToEnd(), ShaderType.Vertex);
        }
        frag.Compile();
        vert.Compile();
        return new (new[]{frag, vert});
    }

}