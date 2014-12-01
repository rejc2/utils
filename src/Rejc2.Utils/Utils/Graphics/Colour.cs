using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rejc2.Utils.Graphics
{
	public struct Colour : IEquatable<Colour>
	{
		public float B;
		public float G;
		public float R;
		public float A;

		public Colour(float r, float g, float b)
			: this(r, g, b, 1f)
		{
		}

		public Colour(double r, double g, double b)
			: this((float)r, (float)g, (float)b, 1f)
		{
		}

		public Colour(float r, float g, float b, float a)
		{
			if (a == 0f)
			{
				B = G = R = A = 0f;
			}
			else
			{
				B = b;
				G = g;
				R = r;
				A = a;
			}
		}

		public static readonly Colour Transparent = new Colour(0f, 0f, 0f, 0f);
		public static readonly Colour White = new Colour(1f, 1f, 1f, 1f);
		public static readonly Colour Black = new Colour(0f, 0f, 0f, 1f);

		public static Colour Interpolate(Colour first, Colour last, float lambda)
		{
			lambda = Math.Max(0f, Math.Min(1f, lambda));
			if (lambda == 0f)
				return first;
			if (lambda == 1f)
				return last;

			float rlambda = 1f - lambda;

			if (first.A == 1f && last.A == 1f)
			{
				return new Colour(
								rlambda * first.R + lambda * last.R,
								rlambda * first.G + lambda * last.G,
								rlambda * first.B + lambda * last.B);
			}
			else
			{
				Colour preMultipliedAlpha = new Colour(
								rlambda * first.R * first.A + lambda * last.R * last.A,
								rlambda * first.G * first.A + lambda * last.G * last.A,
								rlambda * first.B * first.A + lambda * last.B * last.A,
								rlambda * first.A + lambda * last.A);

				if (preMultipliedAlpha.A == 0f)
					return Transparent;

				return new Colour(
								preMultipliedAlpha.R / preMultipliedAlpha.A,
								preMultipliedAlpha.G / preMultipliedAlpha.A,
								preMultipliedAlpha.B / preMultipliedAlpha.A,
								preMultipliedAlpha.A);
			}
		}


		#region IEquatable<Colour> Members

		public bool Equals(Colour other)
		{
			return (A == other.A && R == other.R && G == other.G && B == other.B);
		}

		#endregion

		public override bool Equals(object obj)
		{
			if (!(obj is Colour))
				return false;

			return Equals((Colour)obj);
		}

		public override int GetHashCode()
		{
			return (A.GetHashCode() << 6 ^ R.GetHashCode() << 2 ^ G.GetHashCode() ^ B.GetHashCode() << 4);
		}

		public static bool operator ==(Colour x, Colour y)
		{
			return x.Equals(y);
		}

		public static bool operator !=(Colour x, Colour y)
		{
			return !x.Equals(y);
		}
	}
}
