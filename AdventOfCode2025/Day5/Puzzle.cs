using System.Globalization;
using AdventOfCode2025.Utility;

namespace AdventOfCode2025.Day5;

public class Puzzle : IPuzzleSolver
{
	public DateOnly Date => new DateOnly(2025, 12, 5);

	public (string PartOne, string PartTwo) Solve(string[] input, bool debug)
	{
		//string[] input =
		//[
		//	"3-5",
		//	"10-14",
		//	"16-20",
		//	"12-18",
		//	"",
		//	"1",
		//	"5",
		//	"8",
		//	"11",
		//	"17",
		//	"32",
		//];

		List<(ulong, ulong)> freshFood = [];
		ulong sumOfFreshFood = 0;

		foreach (string l in input)
		{
			int indexOfDash = l.IndexOf('-');

			if (indexOfDash > -1)
			{
				ulong left = ulong.Parse(l[..indexOfDash], NumberStyles.Integer, CultureInfo.InvariantCulture);
				ulong right = ulong.Parse(l[(indexOfDash + 1)..], NumberStyles.Integer, CultureInfo.InvariantCulture);

				freshFood.Add((left, right));

			}
			else if (!string.IsNullOrWhiteSpace(l))
			{
				ulong foodIndex = ulong.Parse(l, NumberStyles.Integer, CultureInfo.InvariantCulture);

				if (freshFood.Any(f => foodIndex >= f.Item1 && foodIndex <= f.Item2))
				{
					if (debug) Console.WriteLine($"Fresh: {foodIndex}");
					sumOfFreshFood++;
				}
				else
				{
					if (debug) Console.WriteLine($"Stale: {foodIndex}");
				}
			}
		}

		if (debug) Console.WriteLine($"Sum of fresh food (part 1): {sumOfFreshFood}");
		
		//second pass for part 2
		
		//sort ranges by start
		List<(ulong, ulong)> sortedRanges = freshFood.OrderBy(r => r.Item1).ToList();
		List<(ulong, ulong)> merged = [];

		//start at 0
		ulong currentStart = sortedRanges[0].Item1;
		ulong currentEnd = sortedRanges[0].Item2;

		//go over all ranges and merge overlapping or contiguous ones
		for (int i = 1; i < sortedRanges.Count; i++)
		{
			ulong nextStart = sortedRanges[i].Item1;
			ulong nextEnd = sortedRanges[i].Item2;

			if (nextStart <= currentEnd + 1) // overlapping or contiguous
			{
				currentEnd = Math.Max(currentEnd, nextEnd);
			}
			else
			{
				merged.Add((currentStart, currentEnd));
				currentStart = nextStart;
				currentEnd = nextEnd;
			}
		}
		//don't forget to add the last range
		merged.Add((currentStart, currentEnd));

		//calculate total number of fresh ingredient IDs
		ulong numberOfIngredientIdsFresh = merged.Aggregate(0UL, (acc, r) => acc + (r.Item2 - r.Item1 + 1));

		if (debug) Console.WriteLine($"Number of ingredient IDs that are fresh (part 2): {numberOfIngredientIdsFresh}");

		return (
			sumOfFreshFood.ToString(CultureInfo.InvariantCulture),
			numberOfIngredientIdsFresh.ToString(CultureInfo.InvariantCulture));
	}
}