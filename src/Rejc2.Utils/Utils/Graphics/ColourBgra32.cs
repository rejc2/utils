using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
//using System.Windows.Media;

namespace Rejc2.Utils.Graphics
{
	[StructLayout(LayoutKind.Explicit)]
	public struct ColourBgra32
	{
		[FieldOffset(0)]
		public byte B;
		[FieldOffset(1)]
		public byte G;
		[FieldOffset(2)]
		public byte R;
		[FieldOffset(3)]
		public byte A;

		public ColourBgra32(byte r, byte g, byte b)
			: this(r, g, b, 255)
		{
		}

		public ColourBgra32(byte r, byte g, byte b, byte a)
		{
			B = b;
			G = g;
			R = r;
			A = a;
		}

		public static ColourBgra32 FromColour(Colour colour)
		{
			return new ColourBgra32(
							(byte)Math.Round(Math.Max(0f, Math.Min(255f, 255f * colour.R))),
							(byte)Math.Round(Math.Max(0f, Math.Min(255f, 255f * colour.G))),
							(byte)Math.Round(Math.Max(0f, Math.Min(255f, 255f * colour.B))),
							(byte)Math.Round(Math.Max(0f, Math.Min(255f, 255f * colour.A))));
		}

		//public static implicit operator ColourBgra32(Color wpfColour)
		//{
		//   return new ColourBgra32(wpfColour.R, wpfColour.G, wpfColour.B, wpfColour.A);
		//}
	}
}
