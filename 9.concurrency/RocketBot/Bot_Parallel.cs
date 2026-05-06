using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace rocket_bot;

public partial class Bot
{
	public Rocket GetNextMove(Rocket rocket)
	{
		var actualThreadsCount = Math.Max(1, Math.Min(threadsCount, iterationsCount));
		var iterationsPerThread = iterationsCount / actualThreadsCount;
		var remainingIterations = iterationsCount % actualThreadsCount;
		var tasks = new Task<(Turn Turn, double Score)>[actualThreadsCount];

		for (var i = 0; i < actualThreadsCount; i++)
		{
			var seed = random.Next();
			var searchIterationsCount = iterationsPerThread + (i < remainingIterations ? 1 : 0);
			tasks[i] = Task.Run(() => SearchBestMove(rocket, new Random(seed), searchIterationsCount));
		}

		var results = Task.WhenAll(tasks)
			.GetAwaiter()
			.GetResult();

		var bestMove = results[0];
		for (var i = 1; i < results.Length; i++)
			if (results[i].Score > bestMove.Score)
				bestMove = results[i];

		return rocket.Move(bestMove.Turn, level);
	}
	
	public List<Task<(Turn Turn, double Score)>> CreateTasks(Rocket rocket)
	{
		return new() { Task.Run(() => SearchBestMove(rocket, new Random(random.Next()), iterationsCount)) };
	}
}
