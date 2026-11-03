using System.Drawing;

namespace MGL.Graphics;

public struct TextStyle(Font font, Color textColor, bool stroked = false, int strokeThickness = 1)
{
    public Font Font { get; set; } = font;
    public Color TextColor { get; set; } = textColor;
    public bool Stroked { get; set; } = stroked;
    public int StrokeThickness { get; set; } = strokeThickness;
}