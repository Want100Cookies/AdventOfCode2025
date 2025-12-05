namespace AdventOfCode2025.Day4;

public static class Puzzle
{
	public static void Execute()
	{
		char[][] input = File.ReadAllLines("Day4/Input.txt")
			.Select(s => s.ToCharArray())
			.ToArray();

		//string[] input =
		//[
		//	"..@@.@@@@.",
		//	"@@@.@.@.@@",
		//	"@@@@@.@.@@",
		//	"@.@@@@..@.",
		//	"@@.@@@@.@@",
		//	".@@@@@@@.@",
		//	".@.@.@.@@@",
		//	"@.@@@.@@@@",
		//	".@@@@@@@@.",
		//	"@.@.@@@.@.",
		//];

		int rowLength = input[0].Length;
		int rolls = 0;
		int currentBatch = 0;

		do
		{
			currentBatch = 0;

			for (int y = 0; y < input.Length; y++)
			{
				for (int x = 0; x < rowLength; x++)
				{
					char current = input[y][x];

					if (current == '@')
					{
						ushort found = 0;

						//check three above
						if (y > 0)
						{
							int i = x;
							if (i > 0) i -= 1;
							for (; i < rowLength && i <= x + 1; i++)
							{
								if (input[y - 1][i] == '@')
								{
									found++;
								}
							}
						}

						//check three below
						if (y < input.Length - 1)
						{
							int i = x;
							if (i > 0) i -= 1;
							for (; i < rowLength && i <= x + 1; i++)
							{
								if (input[y + 1][i] == '@')
								{
									found++;
								}
							}
						}

						//check left
						if (x > 0 && input[y][x - 1] == '@')
						{
							found++;
						}

						//check left
						if (x < rowLength - 1 && input[y][x + 1] == '@')
						{
							found++;
						}

						if (found <= 3)
						{
							current = 'x';
							input[y][x] = '.';
							rolls++;
							currentBatch++;
						}
					}

					//Console.Write(current);
				}

				//Console.WriteLine();
			}

			//Console.Clear();
			Console.WriteLine($"{rolls} - {currentBatch}");

		} while (currentBatch > 0);

		Console.WriteLine($"Rolls found: {rolls}");
	}
}