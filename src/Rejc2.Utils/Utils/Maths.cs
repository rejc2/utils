using System;
using System.Collections.Generic;
using System.Text;

namespace Rejc2.Utils
{
	public static class Maths
	{
		public static double Mod(this double number, double mod)
		{
			var newNumber = number;
			while (newNumber >= mod)
				newNumber -= mod;

			while (newNumber < 0)
				newNumber += mod;

			return newNumber;
		}

		public static float Mod(this float number, float mod)
		{
			var newNumber = number;
			while (newNumber >= mod)
				newNumber -= mod;

			while (newNumber < 0)
				newNumber += mod;

			return newNumber;
		}

		public static int Mod(this int number, int mod)
		{
			var newNumber = number % mod;
			while (newNumber < 0)
				newNumber += mod;

			return newNumber;
		}

		public static double Square(this double number)
		{
			return number * number;
		}
		public static float Square(this float number)
		{
			return number * number;
		}


		public static double MidAngle(double angle1, double angle2, double lambda)
		{
			return MidAngle(angle1, angle2, lambda, false);
		}

		public static double MidAngle(double angle1, double angle2, double lambda, bool inOrder)
		{
			if (inOrder)
			{
				angle2 = angle1 + (angle2 - angle1).Mod(2 * Pi);
			}
			else
			{
				angle1 = angle1.Mod(2 * Pi);
				angle2 = angle2.Mod(2 * Pi);

				if (angle2 - angle1 > Pi)
				{
					angle2 -= 2 * Pi;
				}
				if (angle1 - angle2 > Pi)
				{
					angle2 += 2 * Pi;
				}
			}

			return angle1 * (1 - lambda) + angle2 * lambda;
		}

		public static readonly double Pi = Math.PI;
		public static readonly float PiF = (float)Math.PI;

		public static T Max<T>(T x, T y) where T : IComparable<T>
		{
			if (Comparer<T>.Default.Compare(x, y) < 0)
				return y;
			else
				return x;
		}
		public static T Min<T>(T x, T y) where T : IComparable<T>
		{
			if (Comparer<T>.Default.Compare(x, y) > 0)
				return y;
			else
				return x;
		}

		public static int MixHashInteger(int n)
		{
			uint a = (uint)n;
			a = (a + 0x7ed55d16U) + (a << 12);
			a = (a ^ 0xc761c23cU) ^ (a >> 19);
			a = (a + 0x165667b1U) + (a << 5);
			a = (a + 0xd3a2646cU) ^ (a << 9);
			a = (a + 0xfd7046c5U) + (a << 3);
			a = (a ^ 0xb55a4f09U) ^ (a >> 16);
			return (int)a;
		}
	}
}
