using System;
using System.Collections.Generic;
using System.Text;

namespace AdventOfCode2025.Day1;

public static class Puzzle
{
	public static void Execute()
	{
		string[] rotationStrings = File.ReadAllLines("Day1/Input.txt");

		int currentPoint = 50;
		int howManyAtZeroPartOne = 0;
		int howManyAtZeroPartTwo = 0;

		//Console.WriteLine($"Start: {currentPoint}");

		foreach (string rotationString in rotationStrings)
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

			//Console.WriteLine($"{rotationString} -> {currentPoint}");
		}

		//Console.WriteLine($"End: {currentPoint}");
		Console.WriteLine($"How many at zero (1): {howManyAtZeroPartOne}");
		Console.WriteLine($"How many at zero (2): {howManyAtZeroPartTwo}");
	}

}