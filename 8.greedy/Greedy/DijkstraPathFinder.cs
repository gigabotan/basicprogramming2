using System.Collections.Generic;
using Greedy.Architecture;

namespace Greedy;

class DijkstraData
{
    public Point Previous { get; set; }
    public int Price { get; set; }
}

public class DijkstraPathFinder
{
    private static readonly (int dx, int dy)[] Directions = [(1, 0), (-1, 0), (0, 1), (0, -1)];

    public IEnumerable<PathWithCost> GetPathsByDijkstra(State state, Point start,
        IEnumerable<Point> targets)
    {
        var heap = new PriorityQueue<Point, int>();
        var track = new Dictionary<Point, DijkstraData>();
        var visited = new HashSet<Point>();
        var chests = new HashSet<Point>(state.Chests);

        track[start] = new DijkstraData { Price = 0, Previous = new Point(-1, -1) };
        heap.Enqueue(start, 0);

        while (heap.Count > 0)
        {
            var toOpen = heap.Dequeue();
            if (!visited.Add(toOpen))
                continue;

            if (chests.Contains(toOpen))
            {
                yield return new PathWithCost(track[toOpen].Price, BuildPath(track, toOpen));
                chests.Remove(toOpen);
                if (chests.Count == 0)
                    yield break;
            }

            ProcessNeighbours(state, toOpen, track, visited, heap);
        }
    }

    private static void ProcessNeighbours(State state, Point toOpen,
        Dictionary<Point, DijkstraData> track, HashSet<Point> visited,
        PriorityQueue<Point, int> heap)
    {
        var currentPrice = track[toOpen].Price;
        foreach (var (dx, dy) in Directions)
        {
            var nx = toOpen.X + dx;
            var ny = toOpen.Y + dy;

            if (nx < 0 || ny < 0 || nx >= state.MapWidth || ny >= state.MapHeight)
                continue;

            var neighbour = new Point(nx, ny);
            if (!visited.Contains(neighbour))
                TryUpdateNeighbour(neighbour, currentPrice, toOpen, state, track, heap);
        }
    }

    private static void TryUpdateNeighbour(Point neighbour, int currentPrice, Point from,
        State state, Dictionary<Point, DijkstraData> track, PriorityQueue<Point, int> heap)
    {
        var cellCost = state.CellCost[neighbour.X, neighbour.Y];
        if (cellCost == 0)
            return;

        var newPrice = currentPrice + cellCost;
        if (!track.TryGetValue(neighbour, out var existing) || existing.Price > newPrice)
        {
            track[neighbour] = new DijkstraData { Previous = from, Price = newPrice };
            heap.Enqueue(neighbour, newPrice);
        }
    }

    private static Point[] BuildPath(Dictionary<Point, DijkstraData> track, Point end)
    {
        var stop = new Point(-1, -1);
        var result = new List<Point>();
        for (var current = end; current != stop; current = track[current].Previous)
            result.Add(current);
        result.Reverse();
        return result.ToArray();
    }
}
