using System.Text;
using AdventOfCode2025.Utility;

namespace AdventOfCode2025.Day9;

public class Puzzle : IPuzzleSolver
{
	public DateOnly Date => new DateOnly(2025, 12, 9);

	public (string PartOne, string PartTwo) Solve(string[] input, bool debug)
	{
		//input = [
		//	"7,1",
		//	"11,1",
		//	"11,7",
		//	"9,7",
		//	"9,5",
		//	"2,5",
		//	"2,3",
		//	"7,3"
		//];

		Coordinate[] coordinates = input.Select(x => new Coordinate(x)).ToArray();
		long maxX = coordinates.Max(c => c.X);
		long maxY = coordinates.Max(c => c.Y);

		long largestSurface = CalculateLargestSurface(debug, coordinates);

		if (debug)
		{
			File.WriteAllText("rendered.svg", CreateSvgPolygon(maxX, maxY, coordinates));
		}

		long largestSurfaceWithinCoordinates = CalculateLargestSurfaceWithinCoordinates(debug, coordinates);


		return (largestSurface.ToString(), largestSurfaceWithinCoordinates.ToString());
	}

	private static long CalculateLargestSurface(bool debug, Coordinate[] coordinates)
	{
		long largestSurface = 0;

		for (int i = 0; i < coordinates.Length; i++)
		{
			Coordinate left = coordinates[i];

			for (int j = i + 1; j < coordinates.Length; j++)
			{
				Coordinate right = coordinates[j];

				long surface = GetSurface(left, right);

				if (surface > largestSurface)
				{
					if (debug) Console.WriteLine($"1. New largest surface between {left} and {right} is {surface}");
					largestSurface = surface;
				}
			}
		}

		return largestSurface;
	}

	private static long GetSurface(Coordinate left, Coordinate right)
	{
		long width = Math.Abs((long) left.X - right.X) + 1;
		long height = Math.Abs((long) left.Y - right.Y) + 1;
		long surface = width * height;
		return surface;
	}

	private static long CalculateLargestSurfaceWithinCoordinates(bool debug, Coordinate[] polygon)
	{
		long largestValidSurface = 0;

		//first detect all edges of the polygon
		List<(long Y, long X1, long X2)> horizontalEdges = [];
		List<(long X, long Y1, long Y2)> verticalEdges = [];

		for (int i = 0; i < polygon.Length; i++)
		{
			Coordinate current = polygon[i];
			Coordinate next = polygon[(i + 1) % polygon.Length];

			if (current.Y == next.Y)
			{
				long minX = Math.Min(current.X, next.X);
				long maxX = Math.Max(current.X, next.X);
				horizontalEdges.Add((current.Y, minX, maxX));
			}
			else
			{
				long minY = Math.Min(current.Y, next.Y);
				long maxY = Math.Max(current.Y, next.Y);
				verticalEdges.Add((current.X, minY, maxY));
			}
		}

		//then go over every set of two coordinates
		for (int i = 0; i < polygon.Length; i++)
		{
			Coordinate left = polygon[i];

			for (int j = i + 1; j < polygon.Length; j++)
			{
				Coordinate right = polygon[j];

				long surface = GetSurface(left, right);

				if (surface < largestValidSurface) continue; //no need to check smaller surfaces

				//calculate the four corners of the rectangle
				Coordinate topLeft = new Coordinate(Math.Min(left.X, right.X), Math.Min(left.Y, right.Y));
				Coordinate bottomRight = new Coordinate(Math.Max(left.X, right.X), Math.Max(left.Y, right.Y));

				bool valid = true;

				//check horizontal edges
				foreach ((long Y, long X1, long X2) in horizontalEdges)
				{
					if (topLeft.Y < Y // edge is below the top of the rectangle
					    && bottomRight.Y > Y // edge is above the bottom of the rectangle
					    && bottomRight.X > X1 // edge starts left of the rectangle's right side
					    && topLeft.X < X2) // edge ends right of the rectangle's left side
					{
						valid = false;
						break;
					}
				}

				if (valid)
				{
					//check vertical edges
					foreach ((long X, long Y1, long Y2) in verticalEdges)
					{
						if (topLeft.X < X // edge is right of the rectangle's left side
						    && bottomRight.X > X // edge is left of the rectangle's right side
						    && bottomRight.Y > Y1 // edge starts above the rectangle's bottom
						    && topLeft.Y < Y2) // edge ends below the rectangle's top
						{
							valid = false;
							break;
						}
					}
				}

				if (valid && surface > largestValidSurface)
				{
					largestValidSurface = surface;
					if (debug)
					{
						Console.WriteLine($"2. New largest valid surface between {left} and {right} is {surface}");
					}
				}
			}
		}

		return largestValidSurface;
	}

	private static string CreateSvgPolygon(
		long width,
		long height,
		Coordinate[] points,
		string fill = "none",
		string stroke = "black",
		double strokeWidth = 10.0)
	{
		// Build points attribute: "x1,y1 x2,y2 x3,y3 ..."
		string pointsAttr = string.Join(" ",
			points.Select(p => $"{p.X.ToString(System.Globalization.CultureInfo.InvariantCulture)},{p.Y.ToString(System.Globalization.CultureInfo.InvariantCulture)}"));

		StringBuilder sb = new StringBuilder();
		sb.AppendLine("""<?xml version="1.0" encoding="utf-8"?>""");
		sb.AppendLine($"""<svg xmlns="http://www.w3.org/2000/svg" width="100%" height="100%" viewBox="0 0 {width} {height}" preserveAspectRatio="xMidYMid meet">""");

		// optional: background rect
		sb.AppendLine("""  <rect width="100%" height="100%" fill="white" />""");

		// polygon element
		sb.AppendLine($"""  <polygon points="{pointsAttr}" fill="{fill}" stroke="{stroke}" stroke-width="{strokeWidth}" stroke-linejoin="round" stroke-linecap="round" />""");

		// helpful metadata (optional)
		sb.AppendLine($"  <metadata>Generated by SvgPolygonGenerator on {DateTime.UtcNow:O}</metadata>");

		sb.AppendLine("</svg>");
		return sb.ToString();
	}

	private record Coordinate
	{
		public Coordinate(string s)
		{
			string[] split = s.Split(',');
			X = long.Parse(split[0]);
			Y = long.Parse(split[1]);
		}

		public Coordinate(long x, long y)
		{
			X = x;
			Y = y;
		}

		public long X { get; }
		public long Y { get; }


		public override string ToString()
		{
			return $"({X}, {Y})";
		}
	}
}