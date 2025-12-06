using System.Diagnostics;
using System.Globalization;
using AdventOfCode2025.Utility;

namespace AdventOfCode2025.Day2;

public class Puzzle : IPuzzleSolver
{
	public DateOnly Date => new DateOnly(2025, 12, 2);

	public (string PartOne, string PartTwo) Solve(string[] input, bool debug)
	{
		ReadOnlySpan<char> line = input[0].AsSpan();

		ulong sum = 0;
		ulong sum2 = 0;

		foreach (Range range in line.Split(','))
		{
			int indexOfMinus = range.Start.Value + line[range].IndexOf('-');

			if (indexOfMinus == -1) throw new UnreachableException("Input malformed");

			ReadOnlySpan<char> startString = line[range.Start..indexOfMinus];
			ReadOnlySpan<char> endString = line[(indexOfMinus + 1)..range.End];

			ulong start = ulong.Parse(startString, NumberStyles.None, CultureInfo.InvariantCulture);
			ulong end = ulong.Parse(endString, NumberStyles.None, CultureInfo.InvariantCulture);

			for (ulong value = start; value <= end; value++)
			{
				string valueString = value.ToString(CultureInfo.InvariantCulture);

				PartOne(valueString, value, ref sum);

				PartTwo(valueString, value, ref sum2);
			}
		}

		return (
			sum.ToString(),
			sum2.ToString()
		);
	}

	private static void PartOne(ReadOnlySpan<char> valueString, ulong value, ref ulong sum)
	{
		if (valueString.Length % 2 != 0) return;

		int half = valueString.Length / 2;

		if (valueString[..half].SequenceEqual(valueString[half..]))
		{
			sum += value;
		}
	}

	private static void PartTwo(ReadOnlySpan<char> valueString, ulong value, ref ulong sum)
	{
		for (int i = 1; i <= valueString.Length / 2; i++)
		{
			if (valueString.Length % i != 0)
			{
				//repeating pattern not possible
				continue;
			}

			ReadOnlySpan<char> part = valueString[..i];

			bool anySubpatternMismatch = false;

			for (int j = i; j <= valueString.Length - i; j += i)
			{
				ReadOnlySpan<char> otherPart = valueString[j..(j + i)];

				if (!part.SequenceEqual(otherPart))
				{
					//pattern not repeating
					anySubpatternMismatch = true;
					break;
				}
			}

			if (!anySubpatternMismatch)
			{
				//pattern found
				sum += value;
				return;
			}
		}
	}
}