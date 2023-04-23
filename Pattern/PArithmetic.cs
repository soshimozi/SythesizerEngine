namespace SynthesizerEngine.Pattern;

/// <summary>
/// Arithmetic sequence.
/// Adds a value to a running total on each Next() call.
/// 
/// </summary>
public class PArithmetic : PatternBase
{
    private readonly double _start;
    private readonly object? _step;
    private double _value;
    private readonly int _repeats;
    private int _position;

    public PArithmetic(double start, object? step, int repeats)
    {
        _start = start;
        _step = step;
        _repeats = repeats;
        _value = _start;
        _position = 0;
    }

    public override object? Next()
    {
        object? returnValue;
        if (_position == 0)
        {
            returnValue = _value;
            _position++;
        }
        else if (_position < _repeats)
        {
            var step = ConvertValueToNumber(ValueOf(_step));
            if (step != null)
            {
                _value += step.Value;
                returnValue = _value;
                _position++;
            }
            else
            {
                returnValue = null;
            }
        }
        else
        {
            returnValue = null;
        }

        return (returnValue is PatternBase pb) ? pb.Next() : returnValue;
    }

    public override void Reset()
    {
        _value = _start;
        _position = 0;
        if (_step is PatternBase pb)
        {
            pb.Reset();
        }

    }
}