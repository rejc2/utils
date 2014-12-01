using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace Rejc2.Utils.Graphics
{
	public static class QBitmapUtils
	{
		public static QBitmap<T> FromImage<T>(Image image, Func<Color, T> pixels)
			where T : struct
		{
			if (image == null)
				return null;

			bool dispose;
			Bitmap bitmap;
			if (image is Bitmap)
			{
				bitmap = (Bitmap)image;
				dispose = false;
			}
			else
			{
				bitmap = new Bitmap(image);
				dispose = true;
			}

			var array = new T[bitmap.Width, bitmap.Height];

			for (int x = 0; x < bitmap.Width; x++)
			{
				for (int y = 0; y < bitmap.Height; y++)
				{
					array[x, y] = pixels(bitmap.GetPixel(x, y));
				}
			}

			if (dispose)
			{
				bitmap.Dispose();
				bitmap = null;
			}

			return new QBitmap<T>(array);
		}

		public static QBitmap<bool> BoolFromImage(Image image)
		{
			return FromImage(image, p => p.A != 0);
		}

		public static Bitmap ToImage<T>(this QBitmap<T> qbitmap, Func<T, Color> pixels)
			where T : struct
		{
			Bitmap bitmap = new Bitmap(qbitmap.Width, qbitmap.Height,
										System.Drawing.Imaging.PixelFormat.Format32bppArgb);

			for (int x = 0; x < qbitmap.Width; x++)
			{
				for (int y = 0; y < qbitmap.Height; y++)
				{
					bitmap.SetPixel(x, y, pixels(qbitmap[x,y]));
				}
			}

			return bitmap;
		}
	}
}
