using Silk.NET.OpenGL;

namespace MGL.Graphics
{
	internal class TextVertexArrayObject: IDisposable
	{
		private readonly uint _handle;
		private readonly int _stride;

		private Renderer _renderer;

		public TextVertexArrayObject(Renderer renderer, int stride)
		{
			_renderer = renderer;
			if (stride <= 0)
			{
				throw new ArgumentOutOfRangeException(nameof(stride));
			}

			_stride = stride;

			_renderer.GlContext.GenVertexArrays(1, out _handle);
		}

		public void Dispose()
		{
			_renderer.GlContext.DeleteVertexArray(_handle);
		}

		public void Bind()
		{
			_renderer.GlContext.BindVertexArray(_handle);
		}

		public unsafe void VertexAttribPointer(int location, int size, VertexAttribPointerType type, bool normalized, int offset)
		{
			_renderer.GlContext.EnableVertexAttribArray((uint)location);
			_renderer.GlContext.VertexAttribPointer((uint)location, size, type, normalized, (uint)_stride, (void*)offset);
		}
	}
}
