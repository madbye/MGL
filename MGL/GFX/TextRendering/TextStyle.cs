using System.Drawing;

namespace MGL.GFX.TextRendering;

public struct TextStyle(Font font, Color textColor, bool stroked = false, int strokeThiсkness = 1)
{
    public Font Font { get; set; } = font;
    public Color TextColor { get; set; } = textColor;
    public bool Stroked { get; set; } = stroked;
    public int StrokeThikness { get; set; } = strokeThiсkness;
}