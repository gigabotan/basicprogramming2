using System.Collections.Generic;
using System.Linq;

namespace yield;

public static class ExpSmoothingTask
{
    public static IEnumerable<DataPoint> SmoothExponentialy(this IEnumerable<DataPoint> data, double alpha)
    {
        var prevSmoothed = 0.0;
        var isFirst = true;
        foreach (var point in data)
        {
            if (isFirst)
            {
                prevSmoothed = point.OriginalY;
                isFirst = false;
            }

            prevSmoothed = alpha * point.OriginalY + (1 - alpha) * prevSmoothed;
            yield return point.WithExpSmoothedY(prevSmoothed);
        }
    }
}
