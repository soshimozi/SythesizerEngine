using System.Collections;

namespace SynthesizerEngine.Util;

public class DynamicArray<TKey, TValue> : IEnumerable<KeyValuePair<TKey, TValue>> where TKey : notnull
{
    private readonly Dictionary<TKey, TValue?> _data;
    private readonly TValue? _defaultValue;

    public DynamicArray()
    {
        _data = new Dictionary<TKey, TValue?>();
        _defaultValue = default;
    }

    public DynamicArray(TValue? defaultValue)
    {
        _data = new Dictionary<TKey, TValue?>();
        _defaultValue = defaultValue;
    }

    public TValue? this[TKey index]
    {
        get => _data.TryGetValue(index, out var value) ? value : _defaultValue;
        set
        {
            if (_data.ContainsKey(index))
            {
                _data[index] = value;
            }
            else
            {
                _data.Add(index, value);
            }
        }
    }

    public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
    {
        return _data.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
