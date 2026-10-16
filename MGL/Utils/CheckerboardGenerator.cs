using System.Drawing;
using MGL.Graphics;

namespace MGL.Utils;

public static class CheckerboardGenerator
{
    public static Image GenCheckerboard(Color a, Color b, int size)
    {
        byte[] pixels = new byte[size * size * 3];

        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                bool useColorA = ((x + y) & 1) == 0;
                Color current = useColorA ? a : b;

                int index = (y * size + x) * 3;
                pixels[index]     = current.R;
                pixels[index + 1] = current.G;
                pixels[index + 2] = current.B;
            }
        }

        return new((uint)size, (uint)size, 3, pixels);
    }
}