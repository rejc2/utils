using System;
using System.Collections.Generic;
using System.Text;

namespace Rejc2.Utils
{
	public struct ComplexF : IFormattable, IEquatable<ComplexF>
	{
		public readonly float Re;
		public readonly float Im;

		public ComplexF(float re, float im)
		{
			Re = re;
			Im = im;
		}

		public bool IsZero
		{
			get { return Re == 0f && Im == 0f; }
		}

		public static readonly ComplexF Zero = new ComplexF(0.0F, 0.0F);

		public static readonly ComplexF I = new ComplexF(0f, 1f);

		public static ComplexF operator *(ComplexF c, float a)
		{
			return new ComplexF(a * c.Re, a * c.Im);
		}
		public static ComplexF operator *(float a, ComplexF c)
		{
			return new ComplexF(a * c.Re, a * c.Im);
		}

		public static ComplexF operator /(ComplexF c, float a)
		{
			return new ComplexF(c.Re / a, c.Im / a);
		}

		public static ComplexF operator +(ComplexF a, ComplexF b)
		{
			return new ComplexF(a.Re + b.Re, a.Im + b.Im);
		}

		public static ComplexF operator -(ComplexF a, ComplexF b)
		{
			return new ComplexF(a.Re - b.Re, a.Im - b.Im);
		}

		public static ComplexF operator -(ComplexF a)
		{
			return new ComplexF(-a.Re, -a.Im);
		}

		public static ComplexF operator *(ComplexF a, ComplexF b)
		{
			return new ComplexF(a.Re * b.Re - a.Im * b.Im, a.Re * b.Im + a.Im * b.Re);
		}

		public static ComplexF operator /(ComplexF a, ComplexF b)
		{
			return a * ~b / b.ModulusSquared;
		}

		public float ModulusSquared
		{
			get
			{
				return Re * Re + Im * Im;
			}
		}

		public float Modulus
		{
			get
			{
				return (float)Math.Sqrt(ModulusSquared);
			}
		}

		public static ComplexF Exp(ComplexF x)
		{
			double exp = Math.Exp(x.Re);
			return new ComplexF((float)(exp * Math.Cos(x.Im)), (float)(exp * Math.Sin(x.Im)));
		}

		public static ComplexF operator ~(ComplexF x)
		{
			return new ComplexF(x.Re, -x.Im);
		}

		public override string ToString()
		{
			if (Im == 0f)
				return Re.ToString();
			else
				return String.Format("({0}+{1}i)", Re, Im);
		}

		#region IFormattable Members

		public string ToString(string format, IFormatProvider formatProvider)
		{
			if (Im == 0.0)
				return Re.ToString(formatProvider);
			else
				return String.Format(formatProvider, "({0}+{1}i)", Re.ToString(format, formatProvider),
																	Im.ToString(format, formatProvider));
		}

		#endregion

		#region IEquatable<ComplexF> Members

		public bool Equals(ComplexF other)
		{
			return (Re == other.Re && Im == other.Im);
		}

		#endregion

		public override bool Equals(object obj)
		{
			if (!(obj is ComplexF))
				return false;

			return Equals((ComplexF)obj);
		}

		public override int GetHashCode()
		{
			return Re.GetHashCode() ^ Im.GetHashCode();
		}
	}

}
