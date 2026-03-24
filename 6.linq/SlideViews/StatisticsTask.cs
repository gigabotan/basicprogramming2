using System;
using System.Collections.Generic;
using System.Linq;

namespace linq_slideviews;

public class StatisticsTask
{
	public static double GetMedianTimePerSlide(List<VisitRecord> visits, SlideType slideType)
	{
		var times = visits
			.GroupBy(v => v.UserId)
			.SelectMany(g => g.OrderBy(v => v.DateTime).Bigrams())
			.Where(pair => pair.First.SlideType == slideType)
			.Select(pair => (pair.Second.DateTime - pair.First.DateTime).TotalMinutes)
			.Where(t => t >= 1 && t <= 120)
			.ToList();
		return times.Any() ? times.Median() : 0;
	}
}