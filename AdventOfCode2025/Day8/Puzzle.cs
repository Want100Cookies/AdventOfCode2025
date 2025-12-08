using AdventOfCode2025.Utility;

namespace AdventOfCode2025.Day8;

public class Puzzle : IPuzzleSolver
{
	public DateOnly Date => new DateOnly(2025, 12, 8);

	public (string PartOne, string PartTwo) Solve(string[] input, bool debug)
	{
		//input = [
		//	"162,817,812",
		//	"57,618,57",
		//	"906,360,560",
		//	"592,479,940",
		//	"352,342,300",
		//	"466,668,158",
		//	"542,29,236",
		//	"431,825,988",
		//	"739,650,466",
		//	"52,470,668",
		//	"216,146,977",
		//	"819,987,18",
		//	"117,168,530",
		//	"805,96,715",
		//	"346,949,466",
		//	"970,615,88",
		//	"941,993,340",
		//	"862,61,35",
		//	"984,92,344",
		//	"425,690,689"
		//];

		JunctionBox[] boxes = input.Select(x => new JunctionBox(x)).ToArray();
		Dictionary<(JunctionBox A, JunctionBox B), double> combinations = [];

		foreach (JunctionBox left in boxes)
		{
			foreach (JunctionBox right in boxes)
			{
				if (left == right) continue;

				(JunctionBox, JunctionBox) tuple = left < right ? (left, right) : (right, left);

				if (combinations.ContainsKey(tuple)) continue;

				double distance = left.DistanceTo(right);

				combinations.Add(tuple, distance);
			}
		}

		List<HashSet<JunctionBox>> clustersPartOne = [];

		//int numberOfPairsToConnect = boxes.Length;
		const int numberOfPairsToConnect = 1000;

		ConnectJunctionBoxes(debug, combinations, numberOfPairsToConnect, clustersPartOne);

		ulong multiplication = 1;

		foreach (HashSet<JunctionBox> cluster in clustersPartOne.OrderByDescending(x => x.Count).Take(3))
		{
			multiplication *= (ulong) cluster.Count;

			if(debug) Console.WriteLine($"Cluster ({cluster.Count})");
			foreach (JunctionBox box in cluster)
			{
				if (debug) Console.WriteLine($" - {box}");
			}
		}

		//in part one we connect only a limited number of pairs, in part two we connect all boxes
		ulong multiplicationPartTwo = ConnectJunctionBoxes(debug, combinations, combinations.Count, []);

		return (multiplication.ToString(), multiplicationPartTwo.ToString());
	}

	private static ulong ConnectJunctionBoxes(bool debug, Dictionary<(JunctionBox A, JunctionBox B), double> combinations, int numberOfPairsToConnect, List<HashSet<JunctionBox>> clusters)
	{
		JunctionBox? lastA = null;
		JunctionBox? lastB = null;

		foreach (((JunctionBox a, JunctionBox b), double distance) in combinations.OrderBy(x => x.Value).Take(numberOfPairsToConnect))
		{
			HashSet<JunctionBox>? aCluster = clusters.FirstOrDefault(x => x.Contains(a));
			HashSet<JunctionBox>? bCluster = clusters.FirstOrDefault(x => x.Contains(b));
			
			if (aCluster is null && bCluster is null)
			{
				clusters.Add([a, b]);

				lastA = a;
				lastB = b;
				if (debug) Console.WriteLine($"\t new cluster [{a}, {b}] (total count: {clusters.Count})");
			}
			else if (aCluster is not null && bCluster is null)
			{
				aCluster.Add(b);

				lastA = a;
				lastB = b;
				if (debug) Console.WriteLine($"\t add {b} to cluster of {a} (total count: {clusters.Count})");
			}
			else if (aCluster is null && bCluster is not null)
			{
				bCluster.Add(a);
				
				lastA = a;
				lastB = b;
				if (debug) Console.WriteLine($"\t add {a} to cluster of {b} (total count: {clusters.Count})");
			}
			else if (aCluster != bCluster && aCluster is not null && bCluster is not null)
			{
				aCluster.UnionWith(bCluster);
				clusters.Remove(bCluster);

				lastA = a;
				lastB = b;
				if (debug) Console.WriteLine($"\t merge clusters of {a} and {b} (total count: {clusters.Count})");
			}
			else
			{
				if (debug) Console.WriteLine($"\t {a} and {b} are already in the same cluster");
				continue;
			}

			if (debug) Console.WriteLine($"{a} -> {b} = {distance}");
		}

		if (lastA is not null && lastB is not null)
		{
			ulong multiplication = lastA.X * (ulong) lastB.X;
			if (debug) Console.WriteLine($"{lastA.X} * {lastB.X} = {multiplication}");
			return multiplication;
		}

		//part one does not connect all boxes and thus lastA and lastB can be null
		return 0;
	}

	private record JunctionBox : IComparable<JunctionBox>, IComparable
	{
		public JunctionBox(string s)
		{
			string[] parts = s.Split(',');
			
			X = uint.Parse(parts[0]);
			Y = uint.Parse(parts[1]);
			Z = uint.Parse(parts[2]);
		}

		public uint X { get; }
		public uint Y { get; }
		public uint Z { get; }

		public virtual bool Equals(JunctionBox? other)
		{
			if (other is null) return false;
			if (ReferenceEquals(this, other)) return true;
			return X == other.X && Y == other.Y && Z == other.Z;
		}

		public override int GetHashCode()
		{
			return HashCode.Combine(X, Y, Z);
		}

		public int CompareTo(JunctionBox? other)
		{
			if (ReferenceEquals(this, other)) return 0;
			if (other is null) return 1;
			int xComparison = X.CompareTo(other.X);
			if (xComparison != 0) return xComparison;
			int yComparison = Y.CompareTo(other.Y);
			if (yComparison != 0) return yComparison;
			return Z.CompareTo(other.Z);
		}

		public int CompareTo(object? obj)
		{
			if (obj is null) return 1;
			if (ReferenceEquals(this, obj)) return 0;
			return obj is JunctionBox other ? CompareTo(other) : throw new ArgumentException($"Object must be of type {nameof(JunctionBox)}");
		}

		public static bool operator <(JunctionBox? left, JunctionBox? right)
		{
			return Comparer<JunctionBox>.Default.Compare(left, right) < 0;
		}

		public static bool operator >(JunctionBox? left, JunctionBox? right)
		{
			return Comparer<JunctionBox>.Default.Compare(left, right) > 0;
		}

		public static bool operator <=(JunctionBox? left, JunctionBox? right)
		{
			return Comparer<JunctionBox>.Default.Compare(left, right) <= 0;
		}

		public static bool operator >=(JunctionBox? left, JunctionBox? right)
		{
			return Comparer<JunctionBox>.Default.Compare(left, right) >= 0;
		}

		public double DistanceTo(JunctionBox other)
		{
			return Math.Sqrt(
				Math.Pow(Math.Abs(X - (double) other.X), 2)
				+ Math.Pow(Math.Abs(Y - (double) other.Y), 2)
				+ Math.Pow(Math.Abs(Z - (double) other.Z), 2)
			);
		}
	}
}