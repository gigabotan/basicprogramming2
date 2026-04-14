using System.Collections.Generic;
using System.Linq;
using Greedy.Architecture;

namespace Greedy;

public class NotGreedyPathFinder : IPathFinder
{
    private readonly DijkstraPathFinder dijkstraPathFinder = new();
    private Dictionary<int, Dictionary<int, PathWithCost>> allPaths = new();
    private Dictionary<(int, int), int> bestEnergies = new();
    private List<Point> bestPath = new();
    private int bestCount;

    public List<Point> FindPathToCompleteGoal(State state)
    {
        var chestPoints = state.Chests.Where(point => point != state.Position).ToArray();
        var indexes = GetIndexes(chestPoints);
        var pathPoints = new List<Point>();

        allPaths = BuildAllPaths(state, chestPoints, indexes);
        bestEnergies = new Dictionary<(int, int), int>();
        bestPath = new List<Point>();
        bestCount = state.Chests.Contains(state.Position) ? 1 : 0;
        Search(-1, 0, state.Energy, bestCount, pathPoints);
        return bestPath;
    }

    private Dictionary<Point, int> GetIndexes(Point[] chestPoints)
    {
        var indexes = new Dictionary<Point, int>();

        for (var i = 0; i < chestPoints.Length; i++)
            indexes[chestPoints[i]] = i;

        return indexes;
    }

    private Dictionary<int, Dictionary<int, PathWithCost>> BuildAllPaths(
        State state,
        Point[] chestPoints,
        Dictionary<Point, int> indexes)
    {
        var allPaths = new Dictionary<int, Dictionary<int, PathWithCost>>();
        allPaths[-1] = GetPaths(state, state.Position, chestPoints, indexes);

        for (var i = 0; i < chestPoints.Length; i++)
            allPaths[i] = GetPaths(state, chestPoints[i], chestPoints, indexes);

        return allPaths;
    }

    private Dictionary<int, PathWithCost> GetPaths(
        State state,
        Point start,
        Point[] chestPoints,
        Dictionary<Point, int> indexes)
    {
        var paths = new Dictionary<int, PathWithCost>();

        foreach (var path in dijkstraPathFinder.GetPathsByDijkstra(state, start, chestPoints))
            paths[indexes[path.End]] = path;

        return paths;
    }

    private void Search(
        int currentChest,
        int mask,
        int energy,
        int count,
        List<Point> pathPoints)
    {
        SaveBestPath(count, pathPoints);
        if (IsSeen(currentChest, mask, energy))
            return;

        foreach (var nextChest in allPaths[currentChest].Keys)
            TryNextChest(currentChest, nextChest, mask, energy, count, pathPoints);
    }

    private void SaveBestPath(int count, List<Point> pathPoints)
    {
        if (count <= bestCount)
            return;

        bestCount = count;
        bestPath = new List<Point>(pathPoints);
    }

    private bool IsSeen(int currentChest, int mask, int energy)
    {
        var key = (currentChest, mask);
        if (bestEnergies.TryGetValue(key, out var bestEnergy) && bestEnergy >= energy)
            return true;

        bestEnergies[key] = energy;
        return false;
    }

    private void TryNextChest(
        int currentChest,
        int nextChest,
        int mask,
        int energy,
        int count,
        List<Point> pathPoints)
    {
        if ((mask & (1 << nextChest)) != 0)
            return;

        var path = allPaths[currentChest][nextChest];
        if (path.Cost > energy)
            return;

        var addedPoints = AddPath(pathPoints, path.Path);
        Search(nextChest, mask | (1 << nextChest), energy - path.Cost, count + 1, pathPoints);
        pathPoints.RemoveRange(pathPoints.Count - addedPoints, addedPoints);
    }

    private int AddPath(List<Point> pathPoints, List<Point> nextPathPoints)
    {
        var addedPoints = 0;

        foreach (var point in nextPathPoints.Skip(1))
        {
            pathPoints.Add(point);
            addedPoints++;
        }

        return addedPoints;
    }
}
