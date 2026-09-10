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

			_handle = Env.Gl.GenBuffer();
			
			Bind();

			var elementSizeInBytes = Marshal.SizeOf<T>();
			Env.Gl.BufferData(bufferType, (nuint)(size * elementSizeInBytes), null, isDynamic ? BufferUsageARB.StreamDraw : BufferUsageARB.StaticDraw);
		}

		public void Bind()
		{
			Env.Gl.BindBuffer(_bufferType, _handle);
		}

		public void Dispose()
		{
			Env.Gl.DeleteBuffer(_handle);
		}

		public unsafe void SetData(T[] data, int startIndex, int elementCount)
		{
			Bind();

			fixed(T* dataPtr = &data[startIndex])
			{
				var elementSizeInBytes = sizeof(T);

				Env.Gl.BufferSubData(_bufferType, 0, (nuint)(elementCount * elementSizeInBytes), dataPtr);
			}
		}
	}
}