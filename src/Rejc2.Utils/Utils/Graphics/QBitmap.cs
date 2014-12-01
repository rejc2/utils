using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing; // for Rectangle
using Rejc2.Utils;

namespace Rejc2.Utils.Graphics
{
	public class QBitmap<T> : IEquatable<QBitmap<T>>
					where T : struct
	{
		static readonly EqualityComparer<T> s_Comparer = EqualityComparer<T>.Default;

		private T[,] m_Array;

		int? m_HashCode;

		public QBitmap(T[,] array)
		{
			if (array == null) throw new ArgumentNullException("array");

			m_Array = array;
		}

		public int Width { get { return m_Array.GetLength(0); } }
		public int Height { get { return m_Array.GetLength(1); } }

		public T this[int x, int y]
		{
			get
			{
				return m_Array[x, y];
			}
		}

		public QBitmap<T> SubBitmap(Rectangle rect)
		{
			//if (!new Rectangle(0, 0, Width, Height).Contains(rect))
			//   throw new ArgumentOutOfRangeException("rect", rect, "Should be within: " + Width + "," + Height);
			if (rect.X == 0 && rect.Y == 0 && rect.Width == Width && rect.Height == Height)
				return this;

			T[,] newArray = new T[rect.Width, rect.Height];

			for (int x = 0; x < rect.Width; x++)
			{
				if (rect.X + x < 0)
					continue;
				if (rect.X + x >= Width)
					break;

				for (int y = 0; y < rect.Height; y++)
				{
					if (rect.Y + y < 0)
						continue;
					if (rect.Y + y >= Height)
						break;

					newArray[x, y] = m_Array[rect.X + x, rect.Y + y];
				}
			}

			return new QBitmap<T>(newArray);
		}

		public IEnumerable<T> AllPixels
		{
			get
			{
				foreach (var pixel in m_Array)
					yield return pixel;
			}
		}

		public override bool Equals(object obj)
		{
			return Equals(obj as QBitmap<T>);
		}

		public bool Equals(QBitmap<T> other)
		{
			if ((object)other == null)
				return false;
			if ((object)other == (object)this)
				return true;

			if (m_HashCode.HasValue && other.m_HashCode.HasValue &&
					m_HashCode.Value != other.m_HashCode.Value)
				return false;

			if (other.Width != Width || other.Height != Height)
				return false;

			for (int x = 0; x < Width; x++)
			{
				for (int y = 0; y < Height; y++)
				{
					if (!s_Comparer.Equals(m_Array[x, y], other.m_Array[x, y]))
						return false;
				}
			}

			return true;
		}

		public override int GetHashCode()
		{
			if (m_HashCode.HasValue)
				return m_HashCode.Value;

			int hashCode = 0;
			Random r = new Random((Width << 16) ^ Height);
			foreach (T b in m_Array)
			{
				int next = r.Next();
				hashCode ^= b.GetHashCode() * next;
			}

			m_HashCode = hashCode;

			return hashCode;
		}

		public QBitmap<T> GetTrimmed()
		{
			Rectangle? rect = null;
			for (int x = 0; x < Width; x++)
			{
				for (int y = 0; y < Height; y++)
				{
					if (!s_Comparer.Equals(m_Array[x, y], default(T)))
					{
						var pixelRect = new Rectangle(x, y, 1, 1);
						if (rect.HasValue)
							rect = Rectangle.Union(rect.Value, pixelRect);
						else
							rect = pixelRect;
					}
				}
			}

			return SubBitmap(rect ?? Rectangle.Empty);

		}

		public static bool operator ==(QBitmap<T> x, QBitmap<T> y)
		{
			if ((object)x == (object)y)
				return true;
			if ((object)x == null)
				return false;

			return x.Equals(y);
		}
		public static bool operator !=(QBitmap<T> x, QBitmap<T> y)
		{
			return !(x == y);
		}

		public bool IsBlank
		{
			get
			{
				foreach (var pixel in m_Array)
				{
					if (!s_Comparer.Equals(pixel, default(T)))
						return false;
				}

				return true;
			}
		}

		public void Write(MyBinaryWriter writer, Action<MyBinaryWriter, T> write)
		{
			writer.Write7BitEncodedInt(Width);
			writer.Write7BitEncodedInt(Height);
			for (int x = 0; x < Width; x++)
			{
				for (int y = 0; y < Height; y++)
				{
					write(writer, m_Array[x, y]);
				}
			}
		}

		public static QBitmap<T> Read(MyBinaryReader reader, Func<MyBinaryReader, T> read)
		{
			int width = reader.Read7BitEncodedInt();
			int height = reader.Read7BitEncodedInt();
			T[,] array = new T[width, height];

			for (int x = 0; x < width; x++)
			{
				for (int y = 0; y < height; y++)
				{
					array[x,y] = read(reader);
				}
			}

			return new QBitmap<T>(array);
		}
	}
}
