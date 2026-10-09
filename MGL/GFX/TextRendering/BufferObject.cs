using System.Runtime.InteropServices;
using Silk.NET.OpenGL;

namespace MGL.GFX.TextRendering
{
	internal class BufferObject<T> : IDisposable where T : unmanaged
	{
		private readonly uint _handle;
		private readonly BufferTargetARB _bufferType;
		private readonly int _size;

		public unsafe BufferObject( int size, BufferTargetARB bufferType, bool isDynamic)
		{
			_bufferType = bufferType;
			_size = size;

			_handle = Window.Current.GlContext.GenBuffer();
			
			Bind();

			var elementSizeInBytes = Marshal.SizeOf<T>();
			Window.Current.GlContext.BufferData(bufferType, (nuint)(size * elementSizeInBytes), null, isDynamic ? BufferUsageARB.StreamDraw : BufferUsageARB.StaticDraw);
		}

		public void Bind()
		{
			Window.Current.GlContext.BindBuffer(_bufferType, _handle);
		}

		public void Dispose()
		{
			Window.Current.GlContext.DeleteBuffer(_handle);
		}

		public unsafe void SetData(T[] data, int startIndex, int elementCount)
		{
			Bind();

			fixed(T* dataPtr = &data[startIndex])
			{
				var elementSizeInBytes = sizeof(T);

				Window.Current.GlContext.BufferSubData(_bufferType, 0, (nuint)(elementCount * elementSizeInBytes), dataPtr);
			}
		}
	}
}