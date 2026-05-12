using System.Numerics;

namespace Tickets;

public static class TicketsTask
{
	public static BigInteger Solve(int halfLen, int totalSum)
	{
		if (totalSum % 2 != 0)
			return BigInteger.Zero;

		var halfSum = totalSum / 2;
		if (halfSum < 0 || halfSum > 9 * halfLen)
			return BigInteger.Zero;

		var count = CountNumbersWithSum(halfLen, halfSum);
		return count * count;
	}

	private static BigInteger CountNumbersWithSum(int length, int sum)
	{
		var counts = new BigInteger[sum + 1];
		counts[0] = BigInteger.One;

		for (var digits = 0; digits < length; digits++)
			counts = AddDigit(counts);

		return counts[sum];
	}

	private static BigInteger[] AddDigit(BigInteger[] counts)
	{
		var next = new BigInteger[counts.Length];
		var window = BigInteger.Zero;

		for (var sum = 0; sum < counts.Length; sum++)
		{
			window += counts[sum];
			if (sum > 9)
				window -= counts[sum - 10];

			next[sum] = window;
		}

		return next;
	}
}
