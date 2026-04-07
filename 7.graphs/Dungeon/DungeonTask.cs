using System.Collections.Generic;
using System.Linq;

namespace Dungeon;

public class DungeonTask
{
    public static MoveDirection[] FindShortestPath(Map map)
    {
        if (map == null) return new MoveDirection[0];
        var start = map.InitialPosition;
        var exit = map.Exit;

        var startLists = BfsTask.FindPaths(map, start, map.Chests);
        var exitLists = BfsTask.FindPaths(map, exit, map.Chests);

        if (map.Chests.Any() && startLists.Any() && exitLists.Any())
        {
            var best = SelectBestCandidate(startLists, exitLists, map.Chests);
            if (best != null)
                return ToDirections(best);
        }

        var direct = BfsTask.FindPaths(map, start, new[] { new EmptyChest(exit) })
                            .FirstOrDefault();
        if (direct == null)
            return new MoveDirection[0];
        return ToDirections(direct.Reverse());
    }

    private static IEnumerable<Point> SelectBestCandidate(
        IEnumerable<SinglyLinkedList<Point>> startLists,
        IEnumerable<SinglyLinkedList<Point>> exitLists,
        Chest[] chests)
    {
        var chestByLoc = chests.ToDictionary(c => c.Location, c => c);
        var exitsByLoc = exitLists.ToDictionary(ll => ll.Value, ll => ll);

        var best = startLists
            .Where(s => exitsByLoc.ContainsKey(s.Value))
            .Select(s => (s, exitsByLoc[s.Value]))
            .OrderBy(t => (t.s.Length - 1) + (t.Item2.Length - 1))
            .ThenByDescending(t => chestByLoc.TryGetValue(t.s.Value, out var c) ? c.Value : (byte)0)
            .FirstOrDefault();

        if (best != default)
            return best.s.Reverse().Concat(best.Item2.Skip(1));

        return null;
    }

    private static MoveDirection[] ToDirections(IEnumerable<Point> pts)
    {
        var list = pts.ToList();
        return list
            .Zip(list.Skip(1), (a, b) => b - a)
            .Select(Walker.ConvertOffsetToDirection)
            .ToArray();
    }
}
