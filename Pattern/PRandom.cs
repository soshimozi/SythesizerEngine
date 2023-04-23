using System.Runtime.CompilerServices;

namespace SynthesizerEngine.Pattern;

public class PRandom : PatternBase
{
    private readonly object? _low;
    private readonly object? _high;
    private readonly int _repeats;
    private int _position;
    private readonly Random _random;
    public PRandom(object? low, object? high, int repeats)
    {

        _low = low;
        _high = high;
        _repeats = repeats;
        _position = 0;
        _random = new Random();
    }

    public override object? Next()
    {
        object? returnValue;
        if (_position < _repeats)
        {
            var low = ConvertValueToNumber(ValueOf(_low));
            var high = ConvertValueToNumber(ValueOf(_high));

            //TryConvert(low, out double? lowValue);
            //TryConvert(high, out double? highValue);

            if (low != null && high != null)
            {
                var lowValue = (double?)low;
                var highValue = (double?)high;

                returnValue = low + _random.NextDouble() * (high - low);
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
        _position = 0;
    }
}