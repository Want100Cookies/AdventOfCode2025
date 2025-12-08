using System;
using System.Collections.Generic;
using System.Text;

namespace AdventOfCode2025.Utility;

public interface IPuzzleSolver
{
	public DateOnly Date { get; }

	(string PartOne, string PartTwo) Solve(string[] input, bool debug);
}