namespace SynthesizerEngine.Util;

public class PriorityQueue<T>
{
    private List<T>? Heap { get; set; }
    private readonly Comparison<T> _compare;

    public PriorityQueue(List<T>? array = null, Comparison<T>? compare = null)
    {
        _compare = compare ?? Comparer<T>.Default.Compare;

        if (array != null)
        {
            Heap = array;
            for (var i = 0; i < Heap.Count / 2; i++)
            {
                SiftUp(i);
            }
        }
        else
        {
            Heap = new List<T>();
        }
    }

    public void Push(T item)
    {
        Heap?.Add(item);
        if (Heap != null) SiftDown(0, Heap.Count - 1);
    }

    public T? Pop()
    {
        T returnItem;
        if (Heap == null) return default;

        var lastElement = Heap[^1];
        Heap.RemoveAt(Heap.Count - 1);
        if (Heap.Count > 0)
        {
            returnItem = Heap[0];
            Heap[0] = lastElement;
            SiftUp(0);
        }
        else
        {
            returnItem = lastElement;
        }

        return returnItem;
    }

    public T? Peek()
    {
        return Heap != null ? Heap[0] : default;
    }

    public bool IsEmpty()
    {
        return Heap is { Count: 0 };
    }

    private void SiftDown(int startPosition, int position)
    {
        if (Heap == null) return;
        var newItem = Heap[position];
        while (position > startPosition)
        {
            var parentPosition = (position - 1) >> 1;
            var parent = Heap[parentPosition];
            if (_compare(newItem, parent) < 0)
            {
                Heap[position] = parent;
                position = parentPosition;
                continue;
            }
            break;
        }
        Heap[position] = newItem;
    }

    private void SiftUp(int position)
    {
        if (Heap == null) return;
        var endPosition = Heap.Count;
        var startPosition = position;
        var newItem = Heap[position];
        var childPosition = 2 * position + 1;
        while (childPosition < endPosition)
        {
            var rightPosition = childPosition + 1;
            if (rightPosition < endPosition && _compare(Heap[childPosition], Heap[rightPosition]) >= 0)
            {
                childPosition = rightPosition;
            }
            Heap[position] = Heap[childPosition];
            position = childPosition;
            childPosition = 2 * position + 1;
        }
        Heap[position] = newItem;
        SiftDown(startPosition, position);
    }
}
