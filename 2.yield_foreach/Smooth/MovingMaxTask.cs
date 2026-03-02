using System;
using System.Collections.Generic;
using System.Linq;

namespace yield;

public static class MovingMaxTask
{
    public static IEnumerable<DataPoint> MovingMax(this IEnumerable<DataPoint> data, int windowWidth)
    {
        var slidingWindowQueue = new LinkedList<(double value, int index)>();
        var i = 0;
        foreach (var point in data)
        {
            if (slidingWindowQueue.Count > 0 && slidingWindowQueue.First.Value.index <= i - windowWidth)
                slidingWindowQueue.RemoveFirst();

            while (slidingWindowQueue.Count > 0 && slidingWindowQueue.Last.Value.value <= point.OriginalY)
                slidingWindowQueue.RemoveLast();

            slidingWindowQueue.AddLast((point.OriginalY, i));

            yield return point.WithMaxY(slidingWindowQueue.First.Value.value);
            i++;
        }
    }
}
