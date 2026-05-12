using System.Collections.Generic;

// Каждый документ — это список токенов. То есть List<string>.
// Вместо этого будем использовать псевдоним DocumentTokens.
// Это поможет избежать сложных конструкций:
// вместо List<List<string>> будет List<DocumentTokens>
using DocumentTokens = System.Collections.Generic.List<string>;

namespace Antiplagiarism;

public class LevenshteinCalculator
{
	public List<ComparisonResult> CompareDocumentsPairwise(List<DocumentTokens> documents)
	{
		var comparisons = new List<ComparisonResult>();
		for (var first = 0; first < documents.Count; first++)
		for (var second = first + 1; second < documents.Count; second++)
			comparisons.Add(new ComparisonResult(
				documents[first],
				documents[second],
				GetDistance(documents[first], documents[second])));
		return comparisons;
	}

	private static double GetDistance(DocumentTokens first, DocumentTokens second)
	{
		var distances = new double[first.Count + 1, second.Count + 1];
		for (var i = 0; i <= first.Count; i++)
			distances[i, 0] = i;
		for (var j = 0; j <= second.Count; j++)
			distances[0, j] = j;

		for (var i = 1; i <= first.Count; i++)
		for (var j = 1; j <= second.Count; j++)
		{
			var deletion = distances[i - 1, j] + 1;
			var insertion = distances[i, j - 1] + 1;
			var replacement = distances[i - 1, j - 1]
			                  + TokenDistanceCalculator.GetTokenDistance(first[i - 1], second[j - 1]);
			distances[i, j] = Min(deletion, insertion, replacement);
		}

		return distances[first.Count, second.Count];
	}

	private static double Min(double first, double second, double third)
	{
		if (first <= second && first <= third) return first;
		return second <= third ? second : third;
	}
}
