using System.Collections.Generic;
using System.Linq;

namespace yield;

public static class ExpSmoothingTask
{
    public static IEnumerable<DataPoint> SmoothExponentialy(this IEnumerable<DataPoint> data, double alpha)
    {
        double prevSmoothed = 0;
        bool isFirst = true;
        foreach (var point in data)
        {
            if (isFirst)
            {
                prevSmoothed = point.OriginalY;
                isFirst = false;
            }

            var smoothed = alpha * point.OriginalY + (1 - alpha) * prevSmoothed;
            prevSmoothed = smoothed;
            yield return point.WithExpSmoothedY(smoothed);
        }
    }
}
