using AdventOfCode2025.Utility;

namespace AdventOfCode2025.Day12;

public class Puzzle : IPuzzleSolver
{
	public DateOnly Date => new DateOnly(2025, 12, 12);

	public (string PartOne, string PartTwo) Solve(string[] input, bool debug)
	{
		//demo input part one
		//input =
		//[
		//	"0:",
		//	"###",
		//	"##.",
		//	"##.",
		//	"",
		//	"1:",
		//	"###",
		//	"##.",
		//	".##",
		//	"",
		//	"2:",
		//	".##",
		//	"###",
		//	"##.",
		//	"",
		//	"3:",
		//	"##.",
		//	"###",
		//	"##.",
		//	"",
		//	"4:",
		//	"###",
		//	"#..",
		//	"###",
		//	"",
		//	"5:",
		//	"###",
		//	".#.",
		//	"###",
		//	"",
		//	"4x4: 0 0 0 0 2 0",
		//	"12x5: 1 0 1 0 2 2",
		//	"12x5: 1 0 1 0 3 2",
		//];

		List<Region> regions = [];
		List<Shape> shapes = [];

		//parse shapes manually, always 6 shapes, always 3x3
		for (int i = 0; i < 6; i++)
		{
			List<Point> points = [];
			for (int y = 0; y < 3; y++)
			{
				string line = input[i * 5 + 1 + y];
				for (int x = 0; x < 3; x++)
				{
					if (line[x] == '#')
					{
						points.Add(new Point(x, y));
					}
				}
			}
			shapes.Add(new Shape(i, points));
		}

		foreach (string line in input)
		{
			if (!line.Contains('x')) continue;

			string[] parts = line.Split(':', StringSplitOptions.RemoveEmptyEntries);
			string[] dimensions = parts[0].Split('x', StringSplitOptions.RemoveEmptyEntries);
			int[] requiredShapeCounts = parts[1].Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToArray();
				
			regions.Add(new Region(
				Width: int.Parse(dimensions[0]), 
				Height: int.Parse(dimensions[1]), 
				RequiredShapeCounts: requiredShapeCounts));
		}

		//part one
		int regionsWhichFitAllShapes = 0;
		foreach (Region region in regions)
		{
			List<Shape> shapesToPlace = [];
			for (int shapeId = 0; shapeId < region.RequiredShapeCounts.Length; shapeId++)
			{
				int shapeCount = region.RequiredShapeCounts[shapeId];

				for (int count = 0; count < shapeCount; count++)
				{
					shapesToPlace.Add(shapes.First(s => s.Id == shapeId));
				}
			}

			//check if total points fit in region
			int totalShapePoints = shapesToPlace.Sum(s => s.Points.Count);
			int totalGridPoints = region.Width * region.Height;
			if (totalShapePoints > totalGridPoints)
			{
				if (debug)
				{
					Console.WriteLine($"Region: {region.Width}x{region.Height} {string.Join(',', region.RequiredShapeCounts)} can not fit (too many points)");
				}
				continue;
			}

			//sort shapes by largest first for better pruning
			List<int> indices = Enumerable.Range(0, shapesToPlace.Count)
				.OrderByDescending(i => shapesToPlace[i].Points.Count)
				.ThenBy(i => shapesToPlace[i].Variants.Count)
				.ToList();

			if (debug)
			{
				Console.WriteLine($"Indices: {indices.Count} ");
			}

			bool canPlace = PlaceShapes(region, shapesToPlace, indices, 0, debug);

			if (debug)
			{
				Console.WriteLine($"\nRegion: {region.Width}x{region.Height} {string.Join(',', region.RequiredShapeCounts)} can place {(canPlace ? "yes" : "no")}");
			}

			if (canPlace) regionsWhichFitAllShapes++;
		}

		return ($"Regions: {regionsWhichFitAllShapes}", "No part two :(");
	}

	private static bool PlaceShapes(Region region, List<Shape> shapes, List<int> shapeIndices, int depth, bool debug)
	{
		if (debug && depth % 10 == 0) Console.Write('.');

		//all shapes placed
		if (depth == shapeIndices.Count) return true;

		int shapeIndex = shapeIndices[depth];
		Shape shape = shapes[shapeIndex];
		List<ShapeVariant> variants = shape.Variants;

		//try each variant
		foreach (ShapeVariant variant in variants)
		{
			//try each position
			for (int y = 0; y <= region.Height - variant.Height; y++)
			{
				for (int x = 0; x <= region.Width - variant.Width; x++)
				{
					//check if shape fits and does not overlap
					if (!region.CanPlace(variant, x, y))
					{
						continue;
					}

					//place shape
					region.Set(variant, x, y, true);

					//recurse to place next shape
					if (PlaceShapes(region, shapes, shapeIndices, depth + 1, debug))
					{
						return true;
					}

					//backtrack for next position
					region.Set(variant, x, y, false);
				}
			}
		}

		return false;
	}

	private static List<Point> NormalizeShape(List<Point> points)
	{
		int minX = points.Min(p => p.X);
		int minY = points.Min(p => p.Y);
		return points.Select(p => new Point(p.X - minX, p.Y - minY)).ToList();
	}

	private record Point(int X, int Y);

	private record Shape(int Id, List<Point> Points)
	{
		public List<ShapeVariant> Variants => field ??= GetVariants().ToList();

		private IEnumerable<ShapeVariant> GetVariants()
		{
			HashSet<string> variants = [];

			//make the shape start at 0,0 (edge case)
			List<Point> normalized = NormalizeShape(Points);

			//then for every possible orientation/flip
			List<Func<Point, Point>> transformations =
			[
				//original
				p => p,
				//90 degree
				p => new Point(-p.Y, p.X),
				//180 degree
				p => new Point(-p.X, -p.Y),
				//270 degree
				p => new Point(p.Y, -p.X),
				//flip horizontal
				p => new Point(-p.X, p.Y),
				//filip horizontal + 90 degree
				p => new Point(p.Y, p.X),
				//flip vertical
				p => new Point(p.X, -p.Y),
				//flip vertical + 90 degree
				p => new Point(-p.Y, -p.X),
			];

			foreach (Func<Point, Point> transformation in transformations)
			{
				List<Point> transformed = normalized.Select(transformation).ToList();
				List<Point> normalizedTransformed = NormalizeShape(transformed);

				string hash = string.Join(";", normalizedTransformed.OrderBy(p => p.X).ThenBy(p => p.Y));

				if (!variants.Add(hash)) continue;

				yield return new ShapeVariant(
					Id,
					normalizedTransformed,
					normalizedTransformed.Max(p => p.X) + 1,
					normalizedTransformed.Max(p => p.Y) + 1);
			}
		}
	}

	private record ShapeVariant(int Id, List<Point> Points, int Width, int Height);

	private record Region(int Width, int Height, int[] RequiredShapeCounts)
	{
		private readonly bool[,] State = new bool[Height, Width];

		public bool CanPlace(ShapeVariant variant, int baseX, int baseY)
		{
			foreach (Point variantPoint in variant.Points)
			{
				int x = baseX + variantPoint.X;
				int y = baseY + variantPoint.Y;

				if (x < 0 || x >= Width || y < 0 || y >= Height) return false;
				if (State[y, x]) return false;
			}

			return true;
		}

		public void Set(ShapeVariant variant, int x, int y, bool state)
		{
			foreach (Point variantPoint in variant.Points)
			{
				State[y + variantPoint.Y, x + variantPoint.X] = state;
			}
		}
	};
}