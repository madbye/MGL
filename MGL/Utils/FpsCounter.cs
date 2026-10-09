using System.Diagnostics;

namespace MGL.Utils;

public class FpsCounter
{
    public int FPS { get; private set; }
    public double FrameTimeMs { get; private set; }

    private readonly Stopwatch _stopwatch;
    private long _previousTicks;
    
    private double _timeAccumulator;
    private int _frameCount;

    public FpsCounter()
    {
        _stopwatch = Stopwatch.StartNew();
        _previousTicks = _stopwatch.ElapsedTicks;
    }

    public void Update()
    {
        long currentTicks = _stopwatch.ElapsedTicks;
        long deltaTicks = currentTicks - _previousTicks;
        _previousTicks = currentTicks;
        
        FrameTimeMs = (double)deltaTicks * 1000.0 / Stopwatch.Frequency;

        if (FrameTimeMs > 0)
        {
            _timeAccumulator += FrameTimeMs;
            _frameCount++;

            if (_timeAccumulator >= 500.0)
            {
                FPS = (int)Math.Round(_frameCount * 1000.0 / _timeAccumulator);
                _timeAccumulator = 0;
                _frameCount = 0;
            }
        }
    }
}