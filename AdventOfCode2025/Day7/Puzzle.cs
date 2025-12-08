using System;
using System.Collections.Generic;
using System.Text;
using AdventOfCode2025.Utility;

namespace AdventOfCode2025.Day7;

public class Puzzle : IPuzzleSolver
{
	public DateOnly Date => new DateOnly(2025, 12, 7);

	public (string PartOne, string PartTwo) Solve(string[] input, bool debug)
	{
		//input = [
		//	".......S.......",
		//	"...............",
		//	".......^.......",
		//	"...............",
		//	"......^.^......",
		//	"...............",
		//	".....^.^.^.....",
		//	"...............",
		//	"....^.^...^....",
		//	"...............",
		//	"...^.^...^.^...",
		//	"...............",
		//	"..^...^.....^..",
		//	"...............",
		//	".^.^.^.^.^...^.",
		//	"...............",
		//];

		char[][] manifold = input.Select(x => x.ToCharArray()).ToArray();
		
		int splits = 0;

		PartOneBuildManifold(debug, manifold, ref splits);

		// do a depth first search to get all possible paths
		int beamStart = manifold[0].IndexOf('S');
		ulong possibleWorlds = PartTwoCalculatePossibleWorlds(
			manifold: manifold,
			manifoldCache: new ulong[manifold.Length,manifold[0].Length],
			y: 1, 
			x: beamStart);

		return (splits.ToString(), possibleWorlds.ToString());
	}

	private static void PartOneBuildManifold(bool debug, char[][] manifold, ref int splits)
	{
		for (int y = 0; y < manifold.Length; y++)
		{
			for (int x = 0; x < manifold[y].Length; x++)
			{
				Log(manifold[y][x]);

				if (manifold[y][x] == 'S')
				{
					manifold[y + 1][x] = '|';
				}
				else if (manifold[y][x] == '^' 
				         && manifold[y - 1][x] == '|')
				{
					bool didSplit = false;
					if (manifold[y + 1][x - 1] != '|')
					{
						manifold[y + 1][x - 1] = '|';
						didSplit = true;
					}

					if (manifold[y + 1][x + 1] != '|')
					{
						manifold[y + 1][x + 1] = '|';
						didSplit = true;
					}

					if (didSplit) splits++;
				}
				else if (manifold[y][x] == '|' 
				         && y + 1 < manifold.Length 
				         && manifold[y + 1][x] != '^')
				{
					manifold[y + 1][x] = '|';
				}
			}

			if (debug)
			{
				Console.Write($" ({splits})");
				Console.WriteLine();
			}
		}

		return;

		void Log(char s)
		{
			if (debug)
			{
				Console.Write(s);
				Thread.Sleep(5);
			}
		}
	}

	private static ulong PartTwoCalculatePossibleWorlds(char[][] manifold, ulong[,] manifoldCache, int y, int x)
	{
		while (true)
		{
			if (y == manifold.Length - 1) return 1;

			if (manifoldCache[y, x] != 0) return manifoldCache[y, x];

			if (manifold[y][x] == '^')
			{
				ulong result = PartTwoCalculatePossibleWorlds(manifold, manifoldCache, y + 1, x - 1) //left
				               + PartTwoCalculatePossibleWorlds(manifold, manifoldCache, y + 1, x + 1); //right

				manifoldCache[y, x] = result;
				
				return result;
			}

			y = y + 1;
		}
	}
}