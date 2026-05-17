using System;
using System.Collections;
using System.Collections.Generic;

namespace BinaryTrees;

public class BinaryTree<T> : IEnumerable<T>
	where T : IComparable
{
	private Node root;

	public void Add(T key)
	{
		if (root == null)
		{
			root = new Node(key);
			return;
		}

		var node = FindNodeOrParent(key);
		var comparison = key.CompareTo(node.Key);
		if (comparison == 0)
			return;

		AddChild(node, key, comparison);
		IncreaseCount(key);
	}

	public bool Contains(T key)
	{
		if (root == null)
			return false;
		return key.CompareTo(FindNodeOrParent(key).Key) == 0;
	}

	public T this[int index]
	{
		get
		{
			if (index < 0 || root == null || index >= root.Count)
				throw new IndexOutOfRangeException();
			return GetByIndex(root, index);
		}
	}

	public IEnumerator<T> GetEnumerator()
	{
		return Traverse(root).GetEnumerator();
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return GetEnumerator();
	}

	private static void AddChild(Node node, T key, int comparison)
	{
		if (comparison < 0)
			node.Left = new Node(key);
		else
			node.Right = new Node(key);
	}

	private static T GetByIndex(Node node, int index)
	{
		var leftCount = node.Left?.Count ?? 0;
		if (index < leftCount)
			return GetByIndex(node.Left, index);
		if (index == leftCount)
			return node.Key;
		return GetByIndex(node.Right, index - leftCount - 1);
	}

	private static IEnumerable<T> Traverse(Node node)
	{
		if (node == null)
			yield break;
		foreach (var key in Traverse(node.Left))
			yield return key;
		yield return node.Key;
		foreach (var key in Traverse(node.Right))
			yield return key;
	}

	private Node FindNodeOrParent(T key)
	{
		var current = root;
		var parent = root;
		while (current != null)
		{
			parent = current;
			var comparison = key.CompareTo(current.Key);
			if (comparison == 0)
				return current;
			current = comparison < 0 ? current.Left : current.Right;
		}
		return parent;
	}

	private void IncreaseCount(T key)
	{
		var current = root;
		while (current != null)
		{
			var comparison = key.CompareTo(current.Key);
			if (comparison == 0)
				return;
			current.Count++;
			current = comparison < 0 ? current.Left : current.Right;
		}
	}

	private class Node(T key)
	{
		public readonly T Key = key;
		public Node Left;
		public Node Right;
		public int Count = 1;
	}
}
