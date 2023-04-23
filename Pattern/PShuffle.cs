namespace SynthesizerEngine.Pattern;

public class PShuffle : PatternBase
{
    private int _position;
    private readonly int _repeats;
    private readonly int _offset;
    private readonly List<object?> _list;

    public PShuffle(List<object?> list, int repeats = 1)
    {
        _list = new List<object?>();

        var random = new Random();

        while (list.Count > 0)
        {
            var index = random.Next(0, list.Count);
            var value = list[index];
            list.RemoveRange(index, 1);
            _list.Add(value);
        }

        _repeats = repeats;
        _position = 0;
        _offset = 0;
    }

    public override object? Next()
    {
        object? returnValue;
        if (_position < _repeats * _list.Count)
        {
            var index = (_position + _offset) % _list.Count;
            var item = _list[index];
            var value = ValueOf(item);
            if (value != null)
            {
                if (item is not PatternBase)
                {
                    _position += 1;
                }

                returnValue = value;
            }
            else
            {
                if (item is PatternBase p)
                {
                    p.Reset();
                }
                _position += 1;
                returnValue = Next();
            }
        }
        else
        {
            returnValue = null;
        }

        return (returnValue is PatternBase pb) ? pb.Next() : returnValue;
    }
}