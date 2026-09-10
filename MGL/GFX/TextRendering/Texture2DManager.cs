using System.Drawing;
using FontStashSharp.Interfaces;
using MGL.GFX.Textures;
using Silk.NET.OpenGL;

namespace MGL.GFX.TextRendering
{
	internal class Texture2DManager : ITexture2DManager
	{
		private GL _gl;
		public Texture2DManager(GL gl)
		{
			_gl = gl;
		}

		public object CreateTexture(int width, int height) => new Texture2D((uint)width, (uint)height);

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