using System.Collections.Generic;

namespace yield;

public static class MovingAverageTask
{
    public static IEnumerable<DataPoint> MovingAverage(this IEnumerable<DataPoint> data, int windowWidth)
    {
        var slidingWindowQueue = new Queue<double>();
        var sum = 0.0;
        foreach (var point in data)
        {
            slidingWindowQueue.Enqueue(point.OriginalY);
            sum += point.OriginalY;
            if (slidingWindowQueue.Count > windowWidth)
            {
                sum -= slidingWindowQueue.Dequeue();
            }
            yield return point.WithAvgSmoothedY(sum / slidingWindowQueue.Count);
        }
        yield break;
    }
}
