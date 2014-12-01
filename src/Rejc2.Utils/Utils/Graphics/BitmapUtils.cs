using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;

namespace Rejc2.Utils.Graphics
{
	public static class BitmapUtils
	{
		public static float[][] Greyscale
		{
			get
			{
				return new float[][] {
						new float[] { 0.2126f, 0.2126f, 0.2126f, 0.0f, 0.0f},
						new float[] { 0.7152f, 0.7152f, 0.7152f, 0.0f, 0.0f},
						new float[] { 0.0722f, 0.0722f, 0.0722f, 0.0f, 0.0f},
						new float[] { 0.0f, 0.0f, 0.0f, 1.0f, 0.0f},
						new float[] { 0.0f, 0.0f, 0.0f, 0.0f, 1.0f},
					};
			}
		}

		public static Bitmap ResizeBitmap(this Image image, int size)
		{
			if (image == null) throw new ArgumentNullException("image");

			Bitmap bitmap = new Bitmap(size, size, PixelFormat.Format32bppArgb);
			bitmap.SetResolution(image.HorizontalResolution, image.VerticalResolution);

			using (var g = System.Drawing.Graphics.FromImage(bitmap))
			{
				float newWidth, newHeight;
				if (image.Width > image.Height)
				{
					newWidth = size;
					newHeight = (float)size * image.Height / image.Width;
				}
				else
				{
					newWidth = (float)size * image.Width / image.Height;
					newHeight = size;
				}

				float top = (size - newHeight) / 2f;
				float left = (size - newWidth) / 2f;

				g.DrawImage(image,
						new PointF[] { new PointF(left, top), new PointF(left + newWidth, top), new PointF(left, top + newHeight)
							 },
						new Rectangle(0, 0, image.Width, image.Height),
						GraphicsUnit.Pixel);
			}

			return bitmap;
		}

		public static Bitmap ResizeBitmapSquished(this Image image, int width, int height)
		{
			if (image == null) throw new ArgumentNullException("image");

			Bitmap bitmap = new Bitmap(width, height, PixelFormat.Format32bppArgb);
			bitmap.SetResolution(image.HorizontalResolution, image.VerticalResolution);

			using (var g = System.Drawing.Graphics.FromImage(bitmap))
			{
				float newWidth = width;
				float newHeight = height;

				float top = 0; // (size - newHeight) / 2f;
				float left = 0; // (size - newWidth) / 2f;

				g.DrawImage(image,
						new PointF[] { new PointF(left, top), new PointF(left + newWidth, top), new PointF(left, top + newHeight)
							 },
						new Rectangle(0, 0, image.Width, image.Height),
						GraphicsUnit.Pixel);
			}

			return bitmap;
		}

		public static Bitmap RecolourBitmap(this Image image, float[][] colourMatrix)
		{
			if (image == null) throw new ArgumentNullException("image");

			Bitmap bitmap = new Bitmap(image.Width, image.Height, PixelFormat.Format32bppArgb);
			bitmap.SetResolution(image.HorizontalResolution, image.VerticalResolution);

			using (var g = System.Drawing.Graphics.FromImage(bitmap))
			{
				var matrix = new ColorMatrix(colourMatrix);

				var attr = new ImageAttributes();
				attr.SetColorMatrix(matrix);
				//g.DrawImage(image, RectangleF.FromLTRB(left, top, right, bottom), attr);
				g.DrawImage(image, 
						new Point[] { new Point(0,0), new Point(image.Width, 0), new Point(0, image.Height) },
						new Rectangle(0, 0, image.Width, image.Height),
						GraphicsUnit.Pixel,
						attr);
			}

			return bitmap;
		}
	}
}
