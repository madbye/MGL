using System.Runtime.InteropServices;
using Silk.NET.OpenGL;

namespace MGL.Graphics
{
	internal class TextBufferObject<T> : IDisposable where T : unmanaged
	{
		private readonly uint _handle;
		private readonly BufferTargetARB _bufferType;
		private readonly int _size;

		private Renderer _renderer;

		public unsafe TextBufferObject(Renderer renderer, int size, BufferTargetARB bufferType, bool isDynamic)
		{
			_renderer = renderer;
			
			_bufferType = bufferType;
			_size = size;

			_handle = _renderer.GlContext.GenBuffer();
			
			Bind();

			var elementSizeInBytes = Marshal.SizeOf<T>();
			_renderer.GlContext.BufferData(bufferType, (nuint)(size * elementSizeInBytes), null, isDynamic ? BufferUsageARB.StreamDraw : BufferUsageARB.StaticDraw);
		}

		public void Bind()
		{
			_renderer.GlContext.BindBuffer(_bufferType, _handle);
		}

		public void Dispose()
		{
			_renderer.GlContext.DeleteBuffer(_handle);
		}

		public unsafe void SetData(T[] data, int startIndex, int elementCount)
		{
			Bind();

			fixed(T* dataPtr = &data[startIndex])
			{
				var elementSizeInBytes = sizeof(T);

				_renderer.GlContext.BufferSubData(_bufferType, 0, (nuint)(elementCount * elementSizeInBytes), dataPtr);
			}
		}
	}
}