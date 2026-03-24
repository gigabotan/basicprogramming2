using System;
using System.Collections.Generic;
using System.Linq;

namespace linq_slideviews;

public static class ExtensionsTask
{
	public static double Median(this IEnumerable<double> items)
	{
		var sorted = items.OrderBy(x => x).ToList();
		
		if (sorted.Count == 0)
			throw new InvalidOperationException("Последовательность не содержит элементов");

		int mid = sorted.Count / 2;

		return sorted.Count % 2 == 1
			? sorted[mid]
			: (sorted[mid - 1] + sorted[mid]) / 2.0;
	}

	public static IEnumerable<(T First, T Second)> Bigrams<T>(this IEnumerable<T> items)
	{
		bool hasPrevious = false;
		T previous = default;

		foreach (var current in items)
		{
			if (hasPrevious)
				yield return (First: previous, Second: current);

			hasPrevious = true;
			previous = current;
		}
	}
}