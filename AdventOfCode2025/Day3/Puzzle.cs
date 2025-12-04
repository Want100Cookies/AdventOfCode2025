using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Text;

namespace AdventOfCode2025.Day3;

public static class Puzzle
{
	public static void Execute()
	{
		string[] input = File.ReadAllLines("Day3/Input.txt");
		//string[] input =
		//[
		//	"987654321111111",
		//	"811111111111119",
		//	"234234234234278",
		//	"818181911112111",
		//];

		ulong sum1 = 0;
		ulong sum2 = 0;

		foreach (string line in input)
		{
			ProcessLinePart1(line, ref sum1);
			ProcessLinePart2(line, ref sum2);
		}

		Console.WriteLine($"Sum (part 1): {sum1}");
		Console.WriteLine($"Sum (part 2): {sum2}");
	}

	private static void ProcessLinePart1(ReadOnlySpan<char> line, ref ulong sum)
	{
		int max = -1;
		int secondMax = -1;

		// for every character except the last
		for (int i = 0; i < line.Length - 1; i++)
		{
			// convert char to int
			int n = line[i] - '0';

			// check if n is greater than max
			if (n > max)
			{
				max = n;
				// reset secondMax because the previous one is now invalid
				secondMax = -1;
			}
			// check if n is greater than secondMax
			else if (n > secondMax)
			{
				secondMax = n;
			}
		}

		// check the last character
		if (line[^1] - '0' > secondMax)
		{
			secondMax = line[^1] - '0';
		}

		if (max == -1 || secondMax == -1) throw new UnreachableException("Input malformed");

		int result = ((max * 10) + secondMax);

		//Console.WriteLine($"Input: {line} result 1: {result}");

		sum += (ulong) result;
	}

	private static void ProcessLinePart2(ReadOnlySpan<char> line, ref ulong sum)
	{
		Span<int> maxInts = stackalloc int[12];

		// for every character except the last
		for (int i = 0; i < line.Length - 1; i++)
		{
			// convert char to int
			int n = line[i] - '0';

			var charsLeft = line.Length - i - 1;
			int j = 0;

			// if not enough chars left to fill all 12 positions, start checking from the current length
			if (charsLeft < maxInts.Length)
			{
				j = maxInts.Length - charsLeft - 1;
			}

			for (; j < 12; j++)
			{
				if (n > maxInts[j])
				{
					maxInts[j] = n;

					maxInts[(j + 1)..].Clear(); // reset all positions after j
					break;
				}
			}
		}

		// check the last character and compare it to the last in max ints
		int lastN = line[^1] - '0';
		int lastMax = maxInts[^1];
		if (lastN > lastMax)
		{
			maxInts[^1] = lastN;
		}

		// convert maxInts to an actual number
		double result = 0;
		for (int i1 = 0; i1 < maxInts.Length; i1++)
		{
			result += maxInts[i1] * Math.Pow(10, maxInts.Length - i1 - 1);
		}

		//Console.WriteLine($"Input: {line} result: {result}");

		sum += (ulong) result;
	}
}