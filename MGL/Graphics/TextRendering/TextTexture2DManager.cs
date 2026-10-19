using System.Drawing;
using FontStashSharp.Interfaces;

namespace MGL.Graphics;

internal class TextTexture2DManager : ITexture2DManager, IDisposable
{
    private Renderer _renderer;
    private readonly List<Texture2D> _allocatedTextures = new();

    public TextTexture2DManager(Renderer renderer)
    {
        _renderer = renderer;
    }

    public object CreateTexture(int width, int height)
    {
        var texture = new Texture2D(_renderer, (uint)width, (uint)height);
        _allocatedTextures.Add(texture);
        return texture;
    }

    public Point GetTextureSize(object texture)
    {
        var t = (Texture2D)texture;
        return new Point((int)t.Width, (int)t.Height);
    }

    public void SetTextureData(object texture, Rectangle bounds, byte[] data)
    {
        var t = (Texture2D)texture;
        t.SetData(bounds, data);
    }

    public void Dispose()
    {
        foreach (var texture in _allocatedTextures)
        {
            texture.Dispose(); 
        }
        _allocatedTextures.Clear();
    }
}
