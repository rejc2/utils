using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rejc2.Utils
{
	public static class RandomUtils
	{
		public static int[] TakeN(this Random random, int total, int countToTake)
		{
			if (random == null) throw new ArgumentNullException("random");

			if (total < 0)
				throw new ArgumentOutOfRangeException("total", total, " ");
			if (countToTake < 0)
				throw new ArgumentOutOfRangeException("countToTake", countToTake, " ");
			if (countToTake > total)
				throw new ArgumentOutOfRangeException("countToTake, total");
			
			bool[] taken = new bool[total];
			int[] output = new int[countToTake];

			for (int i = 0; i < countToTake; i++)
			{
				int x = random.Next(0, total - i);
				for (int j = 0; j < total; j++)
				{
					if (taken[j])
						x++;

					if (j == x)
					{
						taken[j] = true;
						output[i] = j;
						break;
					}
				}
			}
			return output;
		}

		public static int WeightedRandom(this Random random, double[] weights)
		{
			if (random == null) throw new ArgumentNullException("random");
			if (weights == null) throw new ArgumentNullException("weights");
			if (weights.Length <= 0)
				throw new ArgumentOutOfRangeException("weights.Length", weights.Length, "Should be >= 1");

			double total = 0;
			for (int i = 0; i < weights.Length; i++)
			{
				total += weights[i];
			}

			double r = random.NextDouble() * total;

			double running = 0;
			for (int i = 0; i < weights.Length - 1; i++)
			{
				running += weights[i];
				if (r <= running)
					return i;
			}

			return weights.Length - 1;
		}
	}
}
