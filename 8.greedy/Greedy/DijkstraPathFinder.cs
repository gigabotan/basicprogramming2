using System;
using System.Collections.Generic;
using System.Linq;
using Greedy.Architecture;

namespace Greedy;

public class DijkstraPathFinder
{
    private static readonly Point[] Directions =
    {
        new(1, 0),
        new(-1, 0),
        new(0, 1),
        new(0, -1)
    };

    public IEnumerable<PathWithCost> GetPathsByDijkstra(
        State state,
        Point start,
        IEnumerable<Point> targets)
    {
        var targetPoints = new HashSet<Point>(targets);
        if (ShouldStop(state, start, targetPoints))
            yield break;

        var pathCosts = new Dictionary<Point, int> { [start] = 0 };
        var previousPoints = new Dictionary<Point, Point>();
        var openPoints = CreateOpenPoints(start);

        while (openPoints.Count > 0 && targetPoints.Count > 0)
            foreach (var path in EnumerateNewPaths(
                         state,
                         start,
                         targetPoints,
                         pathCosts,
                         previousPoints,
                         openPoints))
                yield return path;
    }

    private static bool ShouldStop(
        State state,
        Point start,
        HashSet<Point> targetPoints)
    {
        return targetPoints.Count == 0
               || !state.InsideMap(start)
               || state.IsWallAt(start);
    }

    private static PriorityQueue<Point, int> CreateOpenPoints(Point start)
    {
        var openPoints = new PriorityQueue<Point, int>();
        openPoints.Enqueue(start, 0);
        return openPoints;
    }

    private static IEnumerable<PathWithCost> EnumerateNewPaths(
        State state,
        Point start,
        HashSet<Point> targetPoints,
        Dictionary<Point, int> pathCosts,
        Dictionary<Point, Point> previousPoints,
        PriorityQueue<Point, int> openPoints)
    {
        if (!TryGetCurrentPoint(pathCosts, openPoints, out var currentPoint, out var currentCost))
            yield break;

        if (targetPoints.Remove(currentPoint))
            yield return CreatePath(start, currentPoint, currentCost, previousPoints);

        UpdateNeighbors(state, currentPoint, currentCost, pathCosts, previousPoints, openPoints);
    }

    private static bool TryGetCurrentPoint(
        Dictionary<Point, int> pathCosts,
        PriorityQueue<Point, int> openPoints,
        out Point currentPoint,
        out int currentCost)
    {
        openPoints.TryDequeue(out currentPoint, out currentCost);
        return pathCosts[currentPoint] == currentCost;
    }

    private static PathWithCost CreatePath(
        Point start,
        Point end,
        int cost,
        Dictionary<Point, Point> previousPoints)
    {
        var pathPoints = BuildPath(previousPoints, start, end);
        return new PathWithCost(cost, pathPoints.ToArray());
    }

    private static void UpdateNeighbors(
        State state,
        Point currentPoint,
        int currentCost,
        Dictionary<Point, int> pathCosts,
        Dictionary<Point, Point> previousPoints,
        PriorityQueue<Point, int> openPoints)
    {
        foreach (var nextPoint in GetNeighbors(state, currentPoint))
            UpdatePath(
                state,
                currentPoint,
                currentCost,
                nextPoint,
                pathCosts,
                previousPoints,
                openPoints);
    }

    private static IEnumerable<Point> GetNeighbors(State state, Point point)
    {
        foreach (var direction in Directions)
        {
            var nextPoint = point + direction;
            if (!state.InsideMap(nextPoint) || state.IsWallAt(nextPoint))
                continue;

            yield return nextPoint;
        }
    }

    private static void UpdatePath(
        State state,
        Point currentPoint,
        int currentCost,
        Point nextPoint,
        Dictionary<Point, int> pathCosts,
        Dictionary<Point, Point> previousPoints,
        PriorityQueue<Point, int> openPoints)
    {
        var nextCost = currentCost + state.CellCost[nextPoint.X, nextPoint.Y];
        if (HasNoBetterPath(pathCosts, nextPoint, nextCost))
            return;

        pathCosts[nextPoint] = nextCost;
        previousPoints[nextPoint] = currentPoint;
        openPoints.Enqueue(nextPoint, nextCost);
    }

    private static bool HasNoBetterPath(
        Dictionary<Point, int> pathCosts,
        Point nextPoint,
        int nextCost)
    {
        return pathCosts.TryGetValue(nextPoint, out var knownCost)
               && knownCost <= nextCost;
    }

    private static List<Point> BuildPath(
        Dictionary<Point, Point> previousPoints,
        Point start,
        Point end)
    {
        var pathPoints = new List<Point>();
        var currentPoint = end;

        while (currentPoint != start)
        {
            pathPoints.Add(currentPoint);
            currentPoint = previousPoints[currentPoint];
        }

        pathPoints.Add(start);
        pathPoints.Reverse();
        return pathPoints;
    }
}
