using System;
using System.Collections.Generic;

namespace LimitedSizeStack;

public class LimitedSizeStack<T>
{
    private readonly int maxSize;
    private readonly LinkedList<T> items = new();
    public int Count => items.Count;

    public LimitedSizeStack(int undoLimit)
    {
        maxSize = undoLimit;
    }

    public void Push(T item)
    {
        if (maxSize <= 0)
        {
            return;
        }
        if (Count == maxSize)
        {
            items.RemoveFirst();
        }
        items.AddLast(item);
    }

    public T Pop()
    {
        if (Count == 0)
        {
            throw new InvalidOperationException("Stack is empty.");
        }
        var lastItem = items.Last!.Value;
        items.RemoveLast();
        return lastItem;
    }
}
