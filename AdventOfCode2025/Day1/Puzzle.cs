using AdventOfCode2025.Utility;

namespace AdventOfCode2025.Day1;

public class Puzzle : IPuzzleSolver
{
	public DateOnly Date => new DateOnly(2025, 12, 1);

	public (string PartOne, string PartTwo) Solve(string[] input, bool debug)
	{
		int currentPoint = 50;
		int howManyAtZeroPartOne = 0;
		int howManyAtZeroPartTwo = 0;

		if (debug) Console.WriteLine($"Start: {currentPoint}");

		foreach (string rotationString in input)
		{
			bool isLeft = rotationString[0] == 'L';
			int rotations = int.Parse(rotationString[1..^0]);

			for (int i = 1; i <= rotations; i++)
			{
				if (isLeft)
				{
					currentPoint = currentPoint == 0 ? 99 : currentPoint - 1;
				}
				else
				{
					currentPoint = currentPoint == 99 ? 0 : currentPoint + 1;
				}

				if (currentPoint == 0) howManyAtZeroPartTwo++;
			}

			if (currentPoint == 0) howManyAtZeroPartOne++;

			if (debug) Console.WriteLine($"{rotationString} -> {currentPoint}");
		}

		if (debug) Console.WriteLine($"End: {currentPoint}");

		return (
			howManyAtZeroPartOne.ToString(),
			howManyAtZeroPartTwo.ToString()
		);
	}
}