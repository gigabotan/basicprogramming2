using System;
using System.Collections.Generic;

namespace DiskTree;

public static class DiskTreeTask
{
	public static List<string> Solve(List<string> input)
	{
		var root = new Node();
		foreach (var path in input)
		{
			var current = root;
			foreach (var directory in path.Split('\\'))
			{
				if (!current.Children.TryGetValue(directory, out var child))
				{
					child = new Node();
					current.Children[directory] = child;
				}

				current = child;
			}
		}

		var result = new List<string>();
		AddDirectories(root, 0, result);
		return result;
	}

	private static void AddDirectories(Node node, int depth, List<string> result)
	{
		foreach (var (name, child) in node.Children)
		{
			result.Add(new string(' ', depth) + name);
			AddDirectories(child, depth + 1, result);
		}
	}

	private class Node
	{
		public SortedDictionary<string, Node> Children { get; } = new(StringComparer.Ordinal);
	}
}
