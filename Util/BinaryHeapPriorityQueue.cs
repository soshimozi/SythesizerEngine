namespace SynthesizerEngine.Util;

public class BinaryHeapPriorityQueue<T>
{
    private readonly List<T> _items;
    private readonly Comparison<T>  _comparer;

    public BinaryHeapPriorityQueue(Comparison<T>? comparer = null)
    {
        _items = new List<T>();
        _comparer = comparer ?? Comparer<T>.Default.Compare;
    }

    public int Count => _items.Count;
    public void Enqueue(T item)
    {
        _items.Add(item);
        var index = _items.Count - 1;
        while (index > 0)
        {
            var parentIndex = (index - 1) / 2;
            if (_comparer(_items[parentIndex], _items[index]) <= 0) break;

            Swap(parentIndex, index);
            index = parentIndex;
        }
    }

    public T Dequeue()
    {
        if (_items.Count == 0) throw new InvalidOperationException("The queue is empty.");

        var result = _items[0];
        var lastIndex = _items.Count - 1;
        _items[0] = _items[lastIndex];
        _items.RemoveAt(lastIndex);

        lastIndex -= 1;
        var index = 0;
        while (true)
        {
            var leftChildIndex = 2 * index + 1;
            if (leftChildIndex > lastIndex) break;

            var rightChildIndex = leftChildIndex + 1;
            var smallerChildIndex = (rightChildIndex > lastIndex || _comparer(_items[leftChildIndex], _items[rightChildIndex]) < 0) ? leftChildIndex : rightChildIndex;

            if (_comparer(_items[index], _items[smallerChildIndex]) <= 0) break;

            Swap(index, smallerChildIndex);
            index = smallerChildIndex;
        }

        return result;
    }

    public T Peek()
    {
        if (_items.Count == 0) throw new InvalidOperationException("The queue is empty.");
        return _items[0];
    }

    public bool IsEmpty()
    {
        return _items.Count == 0;
    }

    private void Swap(int index1, int index2)
    {
        (_items[index1], _items[index2]) = (_items[index2], _items[index1]);
    }

    public void Remove(T item)
    {
        var index = _items.IndexOf(item);
        if (index < 0) return;

        var lastIndex = _items.Count - 1;
        _items[index] = _items[lastIndex];
        _items.RemoveAt(lastIndex);

        while (index > 0)
        {
            var parentIndex = (index - 1) / 2;
            if (_comparer(_items[parentIndex], _items[index]) <= 0) break;

            Swap(parentIndex, index);
            index = parentIndex;
        }

        lastIndex -= 1;
        while (true)
        {
            var leftChildIndex = 2 * index + 1;
            if (leftChildIndex > lastIndex) break;

            var rightChildIndex = leftChildIndex + 1;
            var smallerChildIndex = (rightChildIndex > lastIndex || _comparer(_items[leftChildIndex], _items[rightChildIndex]) < 0) ? leftChildIndex : rightChildIndex;

            if (_comparer(_items[index], _items[smallerChildIndex]) <= 0) break;

            Swap(index, smallerChildIndex);
            index = smallerChildIndex;
        }
    }
}
