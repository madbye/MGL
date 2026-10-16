using System.Drawing;
using FontStashSharp.Interfaces;
using Silk.NET.OpenGL;

namespace MGL.Graphics
{
	internal class TextTexture2DManager : ITexture2DManager
	{
		private Renderer _renderer;
		public TextTexture2DManager(Renderer renderer)
		{
			_renderer = renderer;
		}

		public object CreateTexture(int width, int height) => new Texture2D(_renderer, (uint)width, (uint)height);

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
	}
}