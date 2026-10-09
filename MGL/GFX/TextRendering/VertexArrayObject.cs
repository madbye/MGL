using Silk.NET.OpenGL;

namespace MGL.GFX.TextRendering
{
	internal class VertexArrayObject: IDisposable
	{
		private readonly uint _handle;
		private readonly int _stride;

		public VertexArrayObject(int stride)
		{
			if (stride <= 0)
			{
				throw new ArgumentOutOfRangeException(nameof(stride));
			}

			_stride = stride;

			Window.Current.GlContext.GenVertexArrays(1, out _handle);
		}

		public void Dispose()
		{
			Window.Current.GlContext.DeleteVertexArray(_handle);
		}

		public void Bind()
		{
			Window.Current.GlContext.BindVertexArray(_handle);
		}

		public unsafe void VertexAttribPointer(int location, int size, VertexAttribPointerType type, bool normalized, int offset)
		{
			Window.Current.GlContext.EnableVertexAttribArray((uint)location);
			Window.Current.GlContext.VertexAttribPointer((uint)location, size, type, normalized, (uint)_stride, (void*)offset);
		}
	}
}
