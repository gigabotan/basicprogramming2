using System.Collections.Generic;

namespace Antiplagiarism;

public static class LongestCommonSubsequenceCalculator
{
	public static List<string> Calculate(List<string> firstDocument, List<string> secondDocument)
	{
		var longestSubsequenceLengths = CreateOptimizationTable(firstDocument, secondDocument);
		return RestoreAnswer(longestSubsequenceLengths, firstDocument, secondDocument);
	}

	private static int[,] CreateOptimizationTable(List<string> firstDocument, List<string> secondDocument)
	{
		var longestSubsequenceLengths = new int[firstDocument.Count + 1, secondDocument.Count + 1];
		for (var firstLength = 1; firstLength <= firstDocument.Count; firstLength++)
		for (var secondLength = 1; secondLength <= secondDocument.Count; secondLength++)
			if (firstDocument[firstLength - 1] == secondDocument[secondLength - 1])
				longestSubsequenceLengths[firstLength, secondLength]
					= longestSubsequenceLengths[firstLength - 1, secondLength - 1] + 1;
			else
				longestSubsequenceLengths[firstLength, secondLength] = Max(
					longestSubsequenceLengths[firstLength - 1, secondLength],
					longestSubsequenceLengths[firstLength, secondLength - 1]);
		return longestSubsequenceLengths;
	}

	private static List<string> RestoreAnswer(
		int[,] longestSubsequenceLengths,
		List<string> firstDocument,
		List<string> secondDocument)
	{
		var longestSubsequence = new List<string>();
		var firstLength = firstDocument.Count;
		var secondLength = secondDocument.Count;
		while (firstLength > 0 && secondLength > 0)
		{
			if (firstDocument[firstLength - 1] == secondDocument[secondLength - 1])
			{
				longestSubsequence.Add(firstDocument[firstLength - 1]);
				firstLength--;
				secondLength--;
			}
			else if (longestSubsequenceLengths[firstLength - 1, secondLength]
			         >= longestSubsequenceLengths[firstLength, secondLength - 1])
				firstLength--;
			else
				secondLength--;
		}

		longestSubsequence.Reverse();
		return longestSubsequence;
	}

	private static int Max(int firstValue, int secondValue)
	{
		return firstValue >= secondValue ? firstValue : secondValue;
	}
}
