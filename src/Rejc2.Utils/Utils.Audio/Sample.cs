using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace Rejc2.Utils.Audio
{
	[StructLayout(LayoutKind.Explicit)]
	public struct Sample
	{
		[FieldOffset(0)]
		short m_Left;
		public short Left
		{
			get { return m_Left; }
		}

		[FieldOffset(2)]
		short m_Right;
		public short Right
		{
			get { return m_Right; }
		}

		public Sample(short left, short right)
		{
			m_Left = left;
			m_Right = right;
		}

		public Sample(int left, int right)
		{
			m_Left = (short)Math.Max(Math.Min(left, short.MaxValue), short.MinValue);
			m_Right = (short)Math.Max(Math.Min(right, short.MaxValue), short.MinValue);
		}

		public static readonly Sample Zero = new Sample(0, 0);

		public static Sample operator +(Sample x, Sample y)
		{
			return new Sample((int)x.Left + y.Left, (int)x.Right + y.Right);
		}
	}
}
