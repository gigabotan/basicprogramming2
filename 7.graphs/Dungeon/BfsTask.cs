using System.Collections.Generic;

namespace Dungeon;

public class BfsTask
{
    public static IEnumerable<SinglyLinkedList<Point>> FindPaths(Map map, Point start, Chest[] chests)
    {
        if (map == null || chests == null)
            yield break;

        var remainingChests = GetChestLocations(chests);

        foreach (var path in ExplorePaths(map, start, remainingChests))
            yield return path;
    }

    private static IEnumerable<SinglyLinkedList<Point>> ExplorePaths(
        Map map, Point start, HashSet<Point> remainingChests)
    {
        var visitedPoints = new HashSet<Point>();
        var queueItems = new Queue<SinglyLinkedList<Point>>();

        var startNode = new SinglyLinkedList<Point>(start);
        queueItems.Enqueue(startNode);
        visitedPoints.Add(start);

        while (queueItems.Count > 0 && remainingChests.Count > 0)
        {
            var node = queueItems.Dequeue();
            var current = node.Value;

            if (remainingChests.Contains(current))
            {
                yield return node;
                remainingChests.Remove(current);
            }

            foreach (var neighbor in GetValidNeighbors(map, current, visitedPoints))
            {
                visitedPoints.Add(neighbor);
                var nextNode = new SinglyLinkedList<Point>(neighbor, node);
                queueItems.Enqueue(nextNode);
            }
        }
    }

    private static IEnumerable<Point> GetValidNeighbors(
        Map map, Point current, HashSet<Point> visitedPoints)
    {
        foreach (var offset in Walker.PossibleDirections)
        {
            var next = current + offset;
            if (!map.InBounds(next))
                continue;
            if (visitedPoints.Contains(next))
                continue;
            if (map.Dungeon[next.X, next.Y] == MapCell.Wall)
                continue;

            yield return next;
        }
    }

    private static HashSet<Point> GetChestLocations(Chest[] chests)
    {
        var locations = new HashSet<Point>();
        foreach (var chest in chests)
        {
            if (chest?.Location != null)
                locations.Add(chest.Location);
        }

        return locations;
    }
}
