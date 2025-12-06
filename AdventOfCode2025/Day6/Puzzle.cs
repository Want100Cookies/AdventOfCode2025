using System.Globalization;
using AdventOfCode2025.Utility;

namespace AdventOfCode2025.Day6;

public class Puzzle : IPuzzleSolver
{
	public DateOnly Date => new DateOnly(2025, 12, 6);

	public (string PartOne, string PartTwo) Solve(string[] input, bool debug)
	{
		//input =
		//[
		//	"123 328  51 64 ",
		//	" 45 64  387 23 ",
		//	"  6 98  215 314",
		//	"*   +   *   +  "
		//];

		ulong grandTotal = PartOne(input, debug);
		ulong grandTotal2 = PartTwo(input, debug);

		return (
			grandTotal.ToString(),
			grandTotal2.ToString());
	}

	private static ulong PartOne(string[] input, bool debug)
	{
		int[][] numbers = input.SkipLast(1)
			.Select(r =>
				r.Split(' ', StringSplitOptions.RemoveEmptyEntries)
					.Select(x => int.Parse(x, NumberStyles.Integer))
					.ToArray())
			.ToArray();

		char[] operations = input.Last()
			.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
			.Select(x => x[0])
			.ToArray();

		ulong grandTotal = 0;

		for (int col = 0; col < numbers[0].Length; col++)
		{
			ulong result = 0;
			for (int row = 0; row < numbers.Length; row++)
			{
				ulong current = (ulong)numbers[row][col];

				switch (operations[col])
				{
					case '*':
						result = row == 0 ? current : result * current;
						break;
					case '+':
						result += current;
						break;
					default:
						throw new InvalidOperationException("Unknown operation");
				}
			}
			if (debug) Console.WriteLine($"Column {col} sum: {result}");
			grandTotal += result;
		}

		return grandTotal;
	}

	private static ulong PartTwo(string[] input, bool debug)
	{
		//store the group operations for every column
		string lastLine = input[^1];
		char[] groupOperations = new char[lastLine.Length];
		int j = 0;
		for (int i = 0; i < lastLine.Length; i++)
		{
			if (lastLine[i] == '*' || lastLine[i] == '+')
			{
				j = i;
			}

			groupOperations[i] = lastLine[j];
		}

		Span<char> inputColumn = stackalloc char[input.Length - 1];
		
		ulong grandTotal = 0;
		ulong current = 0;

		for (int x = lastLine.Length - 1; x >= 0; x--)
		{
			//reset so that we are not carrying over data
			inputColumn.Fill(' ');

			//extract the column into a decent string
			for (int y = 0; y < input.Length - 1; y++)
			{
				inputColumn[y] = input[y][x];
			}

			if (inputColumn.IsWhiteSpace()) continue;

			//parse the number
			ulong number = ulong.Parse(inputColumn, NumberStyles.Integer, CultureInfo.InvariantCulture);

			//apply the operation for the current group
			if (groupOperations[x] == '*')
			{
				if (debug) Console.WriteLine($"Multiplying {current} by {number}");
				if (current == 0)
				{
					current = number;
				}
				else
				{
					current *= number;
				}
			}
			else if (groupOperations[x] == '+')
			{
				if (debug) Console.WriteLine($"Adding {number} to {current}");
				current += number;
			}

			//check if we continue to the next group
			if (lastLine[x] == ' ') continue;

			if (debug) Console.WriteLine($"Adding current group total {current} to grand total {grandTotal}");
			grandTotal += current;
			current = 0;
		}

		return grandTotal;
	}
}