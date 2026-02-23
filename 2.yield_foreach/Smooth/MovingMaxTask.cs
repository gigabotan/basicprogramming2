using System;
using System.Collections.Generic;
using System.Linq;

namespace yield;

public static class MovingMaxTask
{
    public static IEnumerable<DataPoint> MovingMax(this IEnumerable<DataPoint> data, int windowWidth)
    {
        var queue = new LinkedList<(double value, int index)>();
        int i = 0;
        foreach (var point in data)
        {
            while (queue.Count > 0 && queue.First.Value.index <= i - windowWidth)
                queue.RemoveFirst();

            while (queue.Count > 0 && queue.Last.Value.value <= point.OriginalY)
                queue.RemoveLast();

            queue.AddLast((point.OriginalY, i));

            yield return point.WithMaxY(queue.First.Value.value);
            i++;
        }
    }
}
