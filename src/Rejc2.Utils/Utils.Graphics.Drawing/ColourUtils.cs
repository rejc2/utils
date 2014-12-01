using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace Rejc2.Utils.Graphics.Drawing
{
	public static class ColourUtils
	{
		public static Color ToDrawingColor(this Colour colour)
		{
			return ColourBgra32.FromColour(colour).ToDrawingColor();
		}

		public static Color ToDrawingColor(this ColourBgra32 colour)
		{
			return Color.FromArgb(colour.A, colour.R, colour.G, colour.B);
		}
	}
}
