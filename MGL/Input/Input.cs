using System.Numerics;
using Silk.NET.Input;

namespace MGL.Input;

public class InputContext
{
    public Action<Vector2, MouseButton>? Click;
    public Action<Vector2, MouseButton>? DoubleClick;
    public Action<Vector2>? MouseMove;
    
    public Action<KeyboardKey>? KeyDown;
    public Action<KeyboardKey>? KeyUp;
    public Action<char>? KeyChar;
    
    private IInputContext _inputContext;

    internal InputContext(IInputContext inputContext)
    {
        _inputContext = inputContext;
        _inputContext.Mice[0].Click += (m, button, position) => Click?.Invoke(position, (MouseButton)button);
        _inputContext.Mice[0].DoubleClick += (m, button, position) => DoubleClick?.Invoke(position, (MouseButton)button);
        _inputContext.Mice[0].MouseMove += (m, vec) => MouseMove?.Invoke(vec);
        foreach (var keyboard in _inputContext.Keyboards)
        {
            keyboard.KeyDown += (k, key, arg3) => KeyDown?.Invoke((KeyboardKey)key);
            keyboard.KeyUp += (k, key, arg3) => KeyUp?.Invoke((KeyboardKey)key);
            keyboard.KeyChar += (k, ch) => KeyChar?.Invoke(ch);
        }
    }
    
    public bool IsKeyPressed(KeyboardKey key)
    { 
        foreach (var keyboard in _inputContext.Keyboards)
        {
            if (keyboard.IsKeyPressed((Key)key))
            {
                return true;
            }
        }
        return false;
    }

    public void SetMouseMode(MouseMode mouseMode)
    {
        _inputContext.Mice[0].Cursor.CursorMode = (CursorMode)mouseMode;
    }

    public Vector2 GetMousePosition()
    {
        return _inputContext.Mice[0].Position;
    }

    // public Vector2 GetMouseDelta()
    // {
    //     return _inputContext.Mice[0].
    // }

    public bool GetMouseButton(MouseButton button)
    {
        return _inputContext.Mice[0].IsButtonPressed((Silk.NET.Input.MouseButton)button);
    }
}