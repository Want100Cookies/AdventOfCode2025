using AdventOfCode2025.Utility;
using Google.OrTools.LinearSolver;

namespace AdventOfCode2025.Day10;

public class Puzzle : IPuzzleSolver
{
	public DateOnly Date => new DateOnly(2025, 12, 10);

	public (string PartOne, string PartTwo) Solve(string[] input, bool debug)
	{
		//input = [
		//	"[.##.] (3) (1,3) (2) (2,3) (0,2) (0,1) {3,5,4,7}",
		//	"[...#.] (0,2,3,4) (2,3) (0,4) (0,1,2) (1,2,3,4) {7,5,12,7,2}",
		//	"[.###.#] (0,1,2,3,4) (0,3,4) (0,1,2,4,5) (1,2) {10,11,11,5,10,5}"
		//];

		//part one
		Machine[] machines = input.Select(line => new Machine(line)).ToArray();

		int totalLightButtonsPressed = PressMachineLightButtons(false, machines);
		int totalJoltageButtonsPressed = PressMachineJoltageButtons(debug, machines);

		return (totalLightButtonsPressed.ToString(), totalJoltageButtonsPressed.ToString());
	}

	private static int PressMachineLightButtons(bool debug, Machine[] machines)
	{
		int totalLightButtonsPressed = 0;

		foreach (Machine machine in machines)
		{
			if (debug) Console.Write("\nTesting new machine");

			foreach (IEnumerable<int[]> combination in GetPowerSet(machine.WiringSchematics).OrderBy(x => x.Count()))
			{
				if (debug) Console.Write("\n\tPressing light buttons ");

				int buttonsPressed = 0;

				foreach (int[] wiringSchematic in combination)
				{
					if (debug) Console.Write($"({string.Join(",", wiringSchematic)})");
					buttonsPressed++;
					machine.PressLightButton(wiringSchematic);
				}

				if (machine.AreLightsInCorrectState())
				{
					if (debug) Console.WriteLine($"\n\tFound solution pressing {buttonsPressed} buttons");
					totalLightButtonsPressed += buttonsPressed;
					break;
				}

				machine.ResetLights();
			}
		}

		Console.WriteLine();

		return totalLightButtonsPressed;
	}

	private static int PressMachineJoltageButtons(bool debug, Machine[] machines)
	{
		int totalJoltageButtonsPressed = 0;

		foreach (Machine machine in machines)
		{
			totalJoltageButtonsPressed += PressJoltageButtonForMachine(debug, machine);
		}

		return totalJoltageButtonsPressed;
	}

	private static int PressJoltageButtonForMachine(bool debug, Machine machine)
	{
		//learned something new today: integer linear programming!

		Solver? solver = Solver.CreateSolver("SCIP");
		if (solver == null) throw new Exception("Could not create OR-Tools solver");

		int numSchematics = machine.WiringSchematics.Count;
		int numWires = machine.JoltageRequirements.Length;

		//variables: x[j] = number of times to press schematic j
		Variable[] x = new Variable[numSchematics];
		for (int j = 0; j < numSchematics; j++)
		{
			x[j] = solver.MakeIntVar(0, int.MaxValue, $"x{j}");
		}

		//constraints: for each wire i, sum_j (count of i in schematic j) * x[j] == requirement[i]
		for (int i = 0; i < numWires; i++)
		{
			Constraint? ct = solver.MakeConstraint(machine.JoltageRequirements[i], machine.JoltageRequirements[i]);
			for (int j = 0; j < numSchematics; j++)
			{
				int pressesNeededForRequiredJoltage = machine.WiringSchematics[j].Count(w => w == i);
				if (pressesNeededForRequiredJoltage > 0)
				{
					ct.SetCoefficient(x[j], pressesNeededForRequiredJoltage);
				}
			}
		}

		//objective: minimize total presses
		Objective? objective = solver.Objective();
		for (int j = 0; j < numSchematics; j++)
		{
			objective.SetCoefficient(x[j], 1);
		}
		objective.SetMinimization();

		Solver.ResultStatus resultStatus = solver.Solve();
		if (resultStatus != Solver.ResultStatus.OPTIMAL)
		{
			return -1;
		}

		int totalPresses = 0;
		for (int j = 0; j < numSchematics; j++)
		{
			totalPresses += (int) x[j].SolutionValue();
		}
		return totalPresses;
	}

	//source: https://rosettacode.org/wiki/Power_set#C#:~:text=An%20alternative%20implementation%20for%20an%u2026
	private record Machine
	{
		public Machine(string s)
		{
			foreach (string part in s.Split(' '))
			{
				switch (part[0])
				{
					case '[':
						Lights = part[1..^1].Select(c => new Light(c == '#')).ToArray();
						break;
					case '(':
						WiringSchematics.Add(part[1..^1].Split(',').Select(int.Parse).ToArray());
						break;
					case '{':
						JoltageRequirements = part[1..^1].Split(',').Select(int.Parse).ToArray();
						break;
				}
			}
		}

		public Light[] Lights { get; } = [];
		public List<int[]> WiringSchematics { get; } = [];
		public int[] JoltageRequirements { get; } = [];

		public void PressLightButton(int[] wiring)
		{
			foreach (int lightId in wiring)
			{
				Lights[lightId].State = !Lights[lightId].State;
			}
		}

		public bool AreLightsInCorrectState()
		{
			return Lights.All(light => light.State == light.ExpectedState);
		}

		public void ResetLights()
		{
			foreach (Light light in Lights)
			{
				light.State = false;
			}
		}
	}

	private record Light(bool ExpectedState)
	{
		public bool State { get; set; }
	}

	private record JoltageState(int[] Joltage, int ButtonsPressed)
	{
		public JoltageState PressButton(int[] wiring)
		{
			int[] joltageLevels = Joltage.ToArray();

			foreach (int wire in wiring)
			{
				joltageLevels[wire]++;
			}

			return new JoltageState(joltageLevels, ButtonsPressed + 1);
		}

		public bool IsJoltageRequirementReached(int[] joltageRequirement)
		{
			return Joltage.SequenceEqual(joltageRequirement);
		}

		public bool IsJoltageValid(int[] joltageRequirement)
		{
			for (int i = 0; i < Joltage.Length; i++)
			{
				if (Joltage[i] > joltageRequirement[i])
				{
					return false;
				}
			}

			return true;
		}

		public virtual bool Equals(JoltageState? other)
		{
			if (other is null) return false;
			if (ReferenceEquals(this, other)) return true;
			return Joltage.SequenceEqual(other.Joltage);
		}

		public override int GetHashCode()
		{
			return Joltage.Aggregate(Joltage.Length, (current, val) => unchecked(current * 314159 + val));
		}
	}

	private static IEnumerable<IEnumerable<T>> GetPowerSet<T>(IEnumerable<T> input)
	{
		IEnumerable<IEnumerable<T>> seed = new List<IEnumerable<T>>() { Enumerable.Empty<T>() };

		return input.Aggregate(seed, (a, b) =>
			a.Concat(a.Select(x => x.Concat(new List<T>() { b }))));
	}
}