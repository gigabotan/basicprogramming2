using System.Collections.Generic;

namespace Dungeon;

public class BfsTask
{
    public static IEnumerable<SinglyLinkedList<Point>> FindPaths(Map map, Point start, Chest[] chests)
    {
        if (map == null || chests == null)
            yield break;

        var remainingChests = new HashSet<Point>();
        foreach (var chest in chests)
        {
            if (chest?.Location != null)
                remainingChests.Add(chest.Location);
        }

        var visited = new HashSet<Point>();
        var queue = new Queue<SinglyLinkedList<Point>>();

        var startNode = new SinglyLinkedList<Point>(start);
        queue.Enqueue(startNode);
        visited.Add(start);

        while (queue.Count > 0 && remainingChests.Count > 0)
        {
            var node = queue.Dequeue();
            var current = node.Value;

            if (remainingChests.Contains(current))
            {
                yield return node;
                remainingChests.Remove(current);
            }

            foreach (var offset in Walker.PossibleDirections)
            {
                var next = current + offset;
                if (!map.InBounds(next))
                    continue;
                if (visited.Contains(next))
                    continue;
                if (map.Dungeon[next.X, next.Y] == MapCell.Wall)
                    continue;

                visited.Add(next);
                var nextNode = new SinglyLinkedList<Point>(next, node);
                queue.Enqueue(nextNode);
            }
        }
    }
}
