using System.Reflection;
using AdventOfCode2025.Utility;
using Spectre.Console;

List<IPuzzleSolver> solutions = Assembly.GetExecutingAssembly()
	.GetTypes()
	.Where(t => t.IsAssignableTo(typeof(IPuzzleSolver)) && !t.IsAbstract)
	.Select(t => (IPuzzleSolver) Activator.CreateInstance(t)!)
	.OrderByDescending(t => t.Date)
	.ToList();

IPuzzleSolver solution;

if (args.Length > 0)
{
	solution = solutions.FirstOrDefault(t => t.Date.ToShortDateString() == args[0]) ?? throw new InvalidOperationException("Not found");
}
else
{
	solution = AnsiConsole.Prompt(
		new SelectionPrompt<IPuzzleSolver>()
			.Title("Select the day to calculate")
			.AddChoices(solutions)
			.UseConverter(t => t.Date.ToShortDateString()));
}

bool debug = AnsiConsole.Confirm("Debug output?", false);

string[] input = File.ReadAllLines($"Day{solution.Date.Day}/Input.txt");

(string partOne, string partTwo) = solution.Solve(input, debug);

if (string.IsNullOrWhiteSpace(partOne))
{
	AnsiConsole.MarkupLine("[bold]No solution yet![/]");
}
else
{
	AnsiConsole.MarkupLineInterpolated($"Part one: [bold]{partOne}[/]");
}

if (!string.IsNullOrWhiteSpace(partTwo))
{
	AnsiConsole.MarkupLineInterpolated($"Part two: [bold]{partTwo}[/]");
}