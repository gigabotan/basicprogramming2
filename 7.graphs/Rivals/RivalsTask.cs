using System.Collections.Generic;
using System.Linq;

namespace Rivals;

public class RivalsTask
{
	private static readonly Point[] Directions =
	{
		new(1, 0),
		new(-1, 0),
		new(0, 1),
		new(0, -1)
	};

	public static IEnumerable<OwnedLocation> AssignOwners(Map map)
	{
		var bestOwners = new Dictionary<Point, OwnedLocation>();
		var queue = new Queue<OwnedLocation>();
		var chests = map.Chests.ToHashSet();
		EnqueuePlayers(map, bestOwners, queue);
		ProcessQueue(map, bestOwners, queue, chests);
		return bestOwners.Values;
	}

	private static void EnqueuePlayers(
		Map map,
		Dictionary<Point, OwnedLocation> bestOwners,
		Queue<OwnedLocation> queue)
	{
		for (var owner = 0; owner < map.Players.Length; owner++)
		{
			var start = new OwnedLocation(owner, map.Players[owner], 0);
			bestOwners[start.Location] = start;
			queue.Enqueue(start);
		}
	}

	private static void ProcessQueue(
		Map map,
		Dictionary<Point, OwnedLocation> bestOwners,
		Queue<OwnedLocation> queue,
		HashSet<Point> chests)
	{
		while (queue.Count > 0)
		{
			var current = queue.Dequeue();
			if (bestOwners[current.Location] != current
				|| chests.Contains(current.Location))
				continue;
			TryExpand(map, bestOwners, queue, current);
		}
	}

	private static void TryExpand(
		Map map,
		Dictionary<Point, OwnedLocation> bestOwners,
		Queue<OwnedLocation> queue,
		OwnedLocation current)
	{
		foreach (var direction in Directions)
		{
			var nextPoint = current.Location + direction;
			if (!CanStepOn(map, nextPoint))
				continue;
			TryClaim(bestOwners, queue, current, nextPoint);
		}
	}

	private static void TryClaim(
		Dictionary<Point, OwnedLocation> bestOwners,
		Queue<OwnedLocation> queue,
		OwnedLocation current,
		Point nextPoint)
	{
		var candidate = new OwnedLocation(
			current.Owner,
			nextPoint,
			current.Distance + 1);
		if (bestOwners.TryGetValue(nextPoint, out var currentBest)
			&& !IsBetter(candidate, currentBest))
			return;
		bestOwners[nextPoint] = candidate;
		queue.Enqueue(candidate);
	}

	private static bool CanStepOn(Map map, Point point)
		=> map.InBounds(point)
			&& map.Maze[point.X, point.Y] != MapCell.Wall;

	private static bool IsBetter(OwnedLocation candidate, OwnedLocation currentBest)
	{
		if (candidate.Distance != currentBest.Distance)
			return candidate.Distance < currentBest.Distance;
		return candidate.Owner < currentBest.Owner;
	}
}
