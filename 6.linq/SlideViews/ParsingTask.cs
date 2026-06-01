using System;
using System.Collections.Generic;
using System.Linq;

namespace linq_slideviews;

public class ParsingTask
{
	public static IDictionary<int, SlideRecord> ParseSlideRecords(IEnumerable<string> lines)
	{
		return lines.Skip(1)
			.Select(line => line.Split(';'))
			.Select(parts => TryParseSlideLine(parts))
			.Where(record => record != null)
			.ToDictionary(record => record.SlideId);
	}

	public static IEnumerable<VisitRecord> ParseVisitRecords(
		IEnumerable<string> lines, IDictionary<int, SlideRecord> slides)
	{
		return lines.Skip(1)
			.Select(line => TryParseVisitLine(line, slides))
			.Where(record => record != null);
	}

	private static SlideRecord? TryParseSlideLine(string[] parts)
	{
		if (parts.Length != 3)
			return null;
		if (!int.TryParse(parts[0], out int slideId))
			return null;
		if (!Enum.TryParse<SlideType>(parts[1], true, out SlideType slideType))
			return null;
		return new SlideRecord(slideId, slideType, parts[2]);
	}

	private static VisitRecord? TryParseVisitLine(
		string line, IDictionary<int, SlideRecord> slides)
	{
		var parts = line.Split(';');
		if (parts.Length != 4)
			throw new FormatException($"Wrong line [{line}]");
		if (!int.TryParse(parts[0], out int userId))
			throw new FormatException($"Wrong line [{line}]");
		if (!int.TryParse(parts[1], out int slideId))
			throw new FormatException($"Wrong line [{line}]");
		if (!slides.TryGetValue(slideId, out var slideRecord))
			throw new FormatException($"Wrong line [{line}]");
		if (!DateTime.TryParse($"{parts[2]} {parts[3]}", out var visitDateTime))
			throw new FormatException($"Wrong line [{line}]");
		return new VisitRecord(
			userId, slideId, visitDateTime, slideRecord.SlideType);
	}
}
