using System.Collections.Generic;
using System;

namespace SynthesizerEngine.Pattern;

/// <summary>
/// Choose a random value from an array.
/// </summary>
public class PChoose : PatternBase
{
    private readonly List<object?> _list;
    private readonly int _repeats ;
    private int _position;

    private readonly Random _random;
    public PChoose(List<object?> list, int repeats = 1)
    {
        _list = list;
        _repeats = repeats;
        _position = 0 ;

        _random = new Random();
    }

    public override object? Next()
    {
        object? returnValue;
        if (_position < _repeats)
        {
            var index = _random.Next(0, _list.Count);
            var item = _list[index];
            var value = ValueOf(item);
            if (value != null)
            {
                if (item is not PatternBase) {
                    _position += 1;
                }
                returnValue = value;
            }
            else
            {
                if (item is PatternBase p) {
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

    public override void Reset()
    {
        _position = 0;
        for (var i = 0; i < _list.Count; i++)
        {
            var item = _list[i];
            if (item is PatternBase p) {
                p.Reset();
            }
        }
    }
}