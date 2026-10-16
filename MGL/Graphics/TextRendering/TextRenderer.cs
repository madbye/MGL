using System.Numerics;
using FontStashSharp;
using FontStashSharp.Interfaces;
using Silk.NET.OpenGL;

namespace MGL.Graphics;

public class TextRenderer
{
    private class TRenderer : IFontStashRenderer2, IDisposable
	{
		private const int MAX_SPRITES = 2048;
		private const int MAX_VERTICES = MAX_SPRITES * 4;
		private const int MAX_INDICES = MAX_SPRITES * 6;

		private readonly ShaderProgram _shader;
		private readonly TextBufferObject<VertexPositionColorTexture> _vertexTextBuffer;
		private readonly TextBufferObject<short> _indexTextBuffer;
		private readonly TextVertexArrayObject _vao;
		private readonly VertexPositionColorTexture[] _vertexData = new VertexPositionColorTexture[MAX_VERTICES];
		private object _lastTexture;
		private int _vertexIndex = 0;

		private readonly TextTexture2DManager _textureManager;

		public ITexture2DManager TextureManager => _textureManager;

		private static readonly short[] indexData = GenerateIndexArray();

		private Renderer _renderer;
		
		public unsafe TRenderer(Renderer renderer, ShaderProgram program)
		{
			_renderer = renderer;
			
			_textureManager = new TextTexture2DManager(renderer);

			_vertexTextBuffer = new TextBufferObject<VertexPositionColorTexture>(_renderer, MAX_VERTICES, BufferTargetARB.ArrayBuffer, true);
			_indexTextBuffer = new TextBufferObject<short>(_renderer, indexData.Length, BufferTargetARB.ElementArrayBuffer, false);
			_indexTextBuffer.SetData(indexData, 0, indexData.Length);

			_shader = program;
			_shader.Bind();

			_vao = new TextVertexArrayObject(renderer, sizeof(VertexPositionColorTexture));
			_vao.Bind();

			var location = _shader.GetAttribLocation("a_position");
			_vao.VertexAttribPointer(location, 3, VertexAttribPointerType.Float, false, 0);

			location = _shader.GetAttribLocation("a_color");
			_vao.VertexAttribPointer(location, 4, VertexAttribPointerType.UnsignedByte, true, 12);

			location = _shader.GetAttribLocation("a_texCoords0");
			_vao.VertexAttribPointer(location, 2, VertexAttribPointerType.Float, false, 16);
		}

		~TRenderer() => Dispose(false);
		public void Dispose() => Dispose(true);

		protected virtual void Dispose(bool disposing)
		{
			if (!disposing)
			{
				return;
			}

			_vao.Dispose();
			_vertexTextBuffer.Dispose();
			_indexTextBuffer.Dispose();
			_shader.Dispose();
		}

		internal void Begin(int width, int height)
		{
			_renderer.GlContext.Clear(ClearBufferMask.DepthBufferBit);
			
			_renderer.Disable(EnableCap.DepthTest);
			_renderer.Enable(EnableCap.Blend);

			_shader.Bind();
			_shader.SetUniform("TextureSampler", 0);
			
			var transform = Matrix4x4.CreateOrthographicOffCenter(0, width, height, 0, 0, -1);
			_shader.SetUniform("MatrixTransform", transform);

			_vao.Bind();
			
			_indexTextBuffer.Bind();
			_vertexTextBuffer.Bind();
			
			_renderer.GlContext.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
		}

		public void DrawQuad(object texture, ref VertexPositionColorTexture topLeft, ref VertexPositionColorTexture topRight, ref VertexPositionColorTexture bottomLeft, ref VertexPositionColorTexture bottomRight)
		{
			if (_lastTexture != texture)
			{
				FlushBuffer();
			}

			_vertexData[_vertexIndex++] = topLeft;
			_vertexData[_vertexIndex++] = topRight;
			_vertexData[_vertexIndex++] = bottomLeft;
			_vertexData[_vertexIndex++] = bottomRight;

			_lastTexture = texture;
		}

		public void End()
		{
			FlushBuffer();
		}

		private unsafe void FlushBuffer()
		{
			if (_vertexIndex == 0 || _lastTexture == null)
			{
				return;
			}

			_vertexTextBuffer.SetData(_vertexData, 0, _vertexIndex);

			var texture = (Texture2D)_lastTexture;
			texture.Bind();

			_renderer.GlContext.DrawElements(Silk.NET.OpenGL.PrimitiveType.Triangles, (uint)(_vertexIndex * 6 / 4), DrawElementsType.UnsignedShort, null);
			_vertexIndex = 0;
		}

		private static short[] GenerateIndexArray()
		{
			short[] result = new short[MAX_INDICES];
			for (int i = 0, j = 0; i < MAX_INDICES; i += 6, j += 4)
			{
				result[i] = (short)(j);
				result[i + 1] = (short)(j + 1);
				result[i + 2] = (short)(j + 2);
				result[i + 3] = (short)(j + 3);
				result[i + 4] = (short)(j + 2);
				result[i + 5] = (short)(j + 1);
			}
			return result;
		}

		public void Resize(int width, int height)
		{
			_renderer.GlContext.Viewport(0,0,(uint)width, (uint)height);
    
			_shader.Bind();
			var transform = Matrix4x4.CreateOrthographicOffCenter(0, width, height, 0, 0, -1);
			_shader.SetUniform("MatrixTransform", transform);
		}
	}
    
    private FontSystem _fontSystem = new FontSystem();
    private TRenderer _trenderer;
    private Renderer _renderer;

    public TextRenderer(Renderer renderer, ShaderProgram program)
    {
	    _renderer = renderer;
	    _trenderer = new(_renderer, program);
    }

    public Font LoadFont(string path, int size)
    { 
        _fontSystem.AddFont(File.ReadAllBytes(path));
        var dsfont = _fontSystem.GetFont(size);
        Font font = new();
        font.Handle = dsfont;
        return font;
    }

    private static int width, height;

    public Vector2 MeasureString(string text, TextStyle style, Vector2 scale)
    {
	    return style.Font.Handle.MeasureString(text, scale);
    }
    public void DrawText(string text, TextStyle style, Vector2 position, Vector2? scale = null, float rotation = 0, Vector2? origin = null)
    {
	    bool depth = _renderer.IsEnabled(EnableCap.DepthTest);
	    bool blend = _renderer.IsEnabled(EnableCap.Blend);
	    
	    scale ??= Vector2.One;
	    origin ??= Vector2.Zero;
        
        FSColor color = new FSColor(style.TextColor.R, style.TextColor.G, style.TextColor.B, style.TextColor.A);
        
	    _trenderer.Begin(width, height);
	    
	    if(style.Stroked)
			style.Font.Handle.DrawText(_trenderer, text, position, color, rotation * MathF.PI / 180, origin.Value, scale.Value, effect:FontSystemEffect.Stroked, effectAmount: style.StrokeThikness, characterSpacing: 1);
	    else
		    style.Font.Handle.DrawText(_trenderer, text, position, color, rotation * MathF.PI / 180, origin.Value, scale.Value, characterSpacing: 1);
        _trenderer.End();
        
        if(depth)
	        _renderer.Enable(EnableCap.DepthTest);
        else
	        _renderer.Disable(EnableCap.DepthTest);
        
        if(blend)
	        _renderer.Enable(EnableCap.Blend);
        else
	        _renderer.Disable(EnableCap.Blend);
    }

    public void Resize(uint width, uint height)
    {
        _trenderer.Resize((int)width, (int)height);
        TextRenderer.width = (int)width;
        TextRenderer.height = (int)height;
    }
    
}