using AdventOfCode2025.Utility;

namespace AdventOfCode2025.Day11;

public class Puzzle : IPuzzleSolver
{
	public DateOnly Date => new DateOnly(2025, 12, 11);

	public (string PartOne, string PartTwo) Solve(string[] input, bool debug)
	{
		//demo input part one
		//input = [
		//	"aaa: you hhh",
		//	"you: bbb ccc",
		//	"bbb: ddd eee",
		//	"ccc: ddd eee fff",
		//	"ddd: ggg",
		//	"eee: out",
		//	"fff: out",
		//	"ggg: out",
		//	"hhh: ccc fff iii",
		//	"iii: out",
		//];

		//demo input part two
		//input =
		//[
		//	"svr: aaa bbb",
		//	"aaa: fft",
		//	"fft: ccc",
		//	"bbb: tty",
		//	"tty: ccc",
		//	"ccc: ddd eee",
		//	"ddd: hub",
		//	"hub: fff",
		//	"eee: dac",
		//	"dac: fff",
		//	"fff: ggg hhh",
		//	"ggg: out",
		//	"hhh: out"
		//];

		//parse input into graph
		Dictionary<string, string[]> devices = [];

		foreach (string line in input)
		{
			string device = line[..3];
			string[] outputs = line[5..].Split(' ', StringSplitOptions.RemoveEmptyEntries);

			if (devices.TryGetValue(device, out string[]? existingOutputs))
			{
				devices[device] = existingOutputs.Concat(outputs).Distinct().ToArray();
			}
			else
			{
				devices[device] = outputs;
			}
		}

		ulong possiblePaths1 = GetPossiblePaths("you", "out", devices, []);
		
		if (debug) Console.WriteLine("Got port one");

		ulong svrToFft = GetPossiblePaths("svr", "fft", devices, []);
		ulong fftToDac = GetPossiblePaths("fft", "dac", devices, []);
		ulong dacToOut = GetPossiblePaths("dac", "out", devices, []);
		
		ulong possiblePaths2 = svrToFft * fftToDac * dacToOut;

		return ($"Possible paths from you to out: {possiblePaths1}", $"Possible paths from svr to out via fft and dac: {possiblePaths2}");
	}


	//let's try BFS another time
	private static ulong GetPossiblePaths(string from, string to, Dictionary<string, string[]> devices, Dictionary<string, ulong> cache)
	{
		if (from == to) return 1;

		if (cache.TryGetValue(from, out ulong cachedPaths))
		{
			return cachedPaths;
		}

		ulong totalPaths = 0;

		foreach (string output in devices.GetValueOrDefault(from, []))
		{
			totalPaths += GetPossiblePaths(output, to, devices, cache);
		}

		cache[from] = totalPaths;

		return totalPaths;
	}
}