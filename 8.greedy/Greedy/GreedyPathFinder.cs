using System.Collections.Generic;
using System.Linq;
using Greedy.Architecture;

namespace Greedy;

public class GreedyPathFinder : IPathFinder
{
    public List<Point> FindPathToCompleteGoal(State state)
    {
        var chestPoints = new HashSet<Point>(state.Chests);
        var pathPoints = new List<Point>();
        var currentPoint = state.Position;
        var energy = state.Energy;
        var collectedChests = chestPoints.Remove(currentPoint) ? 1 : 0;

        while (collectedChests < state.Goal)
        {
            var path = FindNextPath(state, currentPoint, chestPoints);
            if (path == null || path.Cost > energy)
                return new List<Point>();

            AppendPath(pathPoints, path.Path);
            energy -= path.Cost;
            currentPoint = path.End;
            if (chestPoints.Remove(currentPoint))
                collectedChests++;
        }

        return pathPoints;
    }

    private static PathWithCost? FindNextPath(
        State state,
        Point currentPoint,
        HashSet<Point> chestPoints)
    {
        var pathFinder = new DijkstraPathFinder();
        return pathFinder
            .GetPathsByDijkstra(state, currentPoint, chestPoints)
            .FirstOrDefault();
    }

    private static void AppendPath(List<Point> pathPoints, List<Point> nextPathPoints)
    {
        foreach (var point in nextPathPoints.Skip(1))
            pathPoints.Add(point);
    }
}
