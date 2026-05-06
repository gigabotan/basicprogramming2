using System.Collections.Generic;

namespace rocket_bot;

public class Channel<T> where T : class
{
    private readonly object locker = new();
    private readonly List<T> items = new();

    public T this[int index]
    {
        get
        {
            lock (locker)
            {
                return index >= 0 && index < items.Count ? items[index] : null;
            }
        }
        set
        {
            lock (locker)
            {
                if (index < 0 || index > items.Count)
                    return;

                if (index == items.Count)
                    items.Add(value);
                else
                {
                    items[index] = value;
                    items.RemoveRange(index + 1, items.Count - index - 1);
                }
            }
        }
    }

    public T LastItem()
    {
        lock (locker)
        {
            return items.Count == 0 ? null : items[^1];
        }
    }


    public void AppendIfLastItemIsUnchanged(T item, T knownLastItem)
    {
        lock (locker)
        {
            if (items.Count == 0 && knownLastItem == null ||
                items.Count > 0 && ReferenceEquals(items[^1], knownLastItem))
                items.Add(item);
        }
    }

    public int Count
    {
        get
        {
            lock (locker)
            {
                return items.Count;
            }
        }
    }
}
