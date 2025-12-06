using AdventOfCode2025.Utility;

namespace AdventOfCode2025.Day4;

public class Puzzle : IPuzzleSolver
{
	public DateOnly Date => new DateOnly(2025, 12, 4);
	public (string PartOne, string PartTwo) Solve(string[] input, bool debug)
	{
		char[][] inputChars = input
			.Select(s => s.ToCharArray())
			.ToArray();

		//string[] input =
		//[
		//	"..@@.@@@@.",
		//	"@@@.@.@.@@",
		//	"@@@@@.@.@@",
		//	"@.@@@@..@.",
		//	"@@.@@@@.@@",
		//	".@@@@@@@.@",
		//	".@.@.@.@@@",
		//	"@.@@@.@@@@",
		//	".@@@@@@@@.",
		//	"@.@.@@@.@.",
		//];

		int? partOne = null;
		int rowLength = inputChars[0].Length;
		int rolls = 0;
		int currentBatch = 0;

		do
		{
			currentBatch = 0;

			for (int y = 0; y < inputChars.Length; y++)
			{
				for (int x = 0; x < rowLength; x++)
				{
					char current = inputChars[y][x];

					if (current == '@')
					{
						ushort found = 0;

						//check three above
						if (y > 0)
						{
							int i = x;
							if (i > 0) i -= 1;
							for (; i < rowLength && i <= x + 1; i++)
							{
								if (inputChars[y - 1][i] == '@')
								{
									found++;
								}
							}
						}

						//check three below
						if (y < inputChars.Length - 1)
						{
							int i = x;
							if (i > 0) i -= 1;
							for (; i < rowLength && i <= x + 1; i++)
							{
								if (inputChars[y + 1][i] == '@')
								{
									found++;
								}
							}
						}

						//check left
						if (x > 0 && inputChars[y][x - 1] == '@')
						{
							found++;
						}

						//check left
						if (x < rowLength - 1 && inputChars[y][x + 1] == '@')
						{
							found++;
						}

						if (found <= 3)
						{
							current = 'x';
							inputChars[y][x] = '.';
							rolls++;
							currentBatch++;
						}
					}

					if (debug) Console.Write(current);
				}

				if (debug) Console.WriteLine();
			}

			if (debug) Console.Clear();
			partOne ??= currentBatch;
			if (debug) Console.WriteLine($"{rolls} - {currentBatch}");

		} while (currentBatch > 0);

		if (debug) Console.WriteLine($"Rolls found: {rolls}");
		return (
			"not sure how I figured this one out, but part two broke it :)",
			rolls.ToString()
		);
	}
}