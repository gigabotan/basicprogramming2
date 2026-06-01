using System;
using System.Collections.Generic;
using System.Linq;

namespace linq_slideviews;

public class StatisticsTask
{
	public static double GetMedianTimePerSlide(List<VisitRecord> visits, SlideType slideType)
	{
		var durationMinutes = visits
			.GroupBy(visit => visit.UserId)
			.SelectMany(userVisits => userVisits.OrderBy(visit => visit.DateTime).Bigrams())
			.Where(visitPair => visitPair.First.SlideType == slideType)
			.Select(visitPair => (visitPair.Second.DateTime - visitPair.First.DateTime).TotalMinutes)
			.Where(duration => duration >= 1 && duration <= 120)
			.ToList();
		return durationMinutes.Count > 0 ? durationMinutes.Median() : 0;
	}
}