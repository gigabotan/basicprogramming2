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
			.Where(parts => parts.Length == 3 && int.TryParse(parts[0], out _) && Enum.TryParse<SlideType>(parts[1], true, out _))
			.Select(parts => new SlideRecord(int.Parse(parts[0]), Enum.Parse<SlideType>(parts[1], true), parts[2]))
			.ToDictionary(r => r.SlideId);
	}


	public static IEnumerable<VisitRecord> ParseVisitRecords(
		IEnumerable<string> lines, IDictionary<int, SlideRecord> slides)
	{
		return lines.Skip(1)
			.Select(line =>
			{
				var parts = line.Split(';');
				if (parts.Length != 4) throw new FormatException("Wrong line [" + line + "]");
				if (!int.TryParse(parts[0], out int userId)) throw new FormatException("Wrong line [" + line + "]");
				if (!int.TryParse(parts[1], out int slideId)) throw new FormatException("Wrong line [" + line + "]");
				if (!slides.TryGetValue(slideId, out var slideRecord)) throw new FormatException("Wrong line [" + line + "]");
				if (!DateTime.TryParse(parts[2] + " " + parts[3], out var dateTime)) throw new FormatException("Wrong line [" + line + "]");
				return new VisitRecord(userId, slideId, dateTime, slideRecord.SlideType);
			});
	}
}