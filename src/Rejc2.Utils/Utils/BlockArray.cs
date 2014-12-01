using System;
using System.Collections.Generic;
using System.Text;

namespace Rejc2.Utils
{
	interface ConversionFunction<T, S>
	{
		S Function(T value);
	}

	public class BlockArray<T> : IEnumerable<T>
	{
		readonly object m_WriterLock = new object();

		readonly int m_BlockSizeLog2;
		readonly int m_BlockSize;
		readonly T[][] m_Blocks;

		volatile int m_Length;

		public BlockArray()
			: this(15)
		{ }

		public BlockArray(int blockSizeLog2)
			: this(blockSizeLog2, (int)(2048U * 1024 * 1024 / (1 << blockSizeLog2)))
		{ }

		public BlockArray(int blockSizeLog2, int blocks)
		{
			m_BlockSizeLog2 = blockSizeLog2;
			m_BlockSize = 1 << blockSizeLog2;
			m_Blocks = new T[blocks][];
		}

		public void Write(T[] data)
		{
			Write(data, 0, data.Length);
		}

		public void Write(T[] data, int offset, int count)
		{
			if (data == null) throw new ArgumentNullException("data");
			if (offset < 0) throw new ArgumentOutOfRangeException("offset");
			if (count < 0) throw new ArgumentOutOfRangeException("count");
			if (offset + count > data.Length) throw new ArgumentOutOfRangeException();

			lock (m_WriterLock)
			{
				int length = m_Length;
				while (count > 0)
				{
					int blockIndex = length >> m_BlockSizeLog2;
					T[] block = m_Blocks[blockIndex];
					if (block == null)
						block = m_Blocks[blockIndex] = new T[m_BlockSize];

					int withinBlock = length & (m_BlockSize - 1);
					length += Math.Min(count, m_BlockSize - withinBlock);
					for (; count > 0 && withinBlock < m_BlockSize; withinBlock++, offset ++, count--)
					{
						block[withinBlock] = data[offset];
					}
				}

				m_Length = length;
			}
		}

		public void Read(int readOffset, T[] buffer, int bufferOffset, int count)
		{
			if (buffer == null) throw new ArgumentNullException("buffer");
			if (readOffset < 0) throw new ArgumentOutOfRangeException("readOffset");
			if (count < 0) throw new ArgumentOutOfRangeException("count");
			if (bufferOffset < 0) throw new ArgumentOutOfRangeException("bufferOffset");
			if (readOffset + count > m_Length) throw new ArgumentOutOfRangeException();
			if (bufferOffset + count > buffer.Length) throw new ArgumentOutOfRangeException();

			while (count > 0)
			{
				int blockIndex = readOffset >> m_BlockSizeLog2;
				T[] block = m_Blocks[blockIndex];
				if (block == null) throw new InvalidOperationException();

				int withinBlock = readOffset & (m_BlockSize - 1);
				readOffset += Math.Min(count, m_BlockSize - withinBlock);
				for (; count > 0 && withinBlock < m_BlockSize; withinBlock++, bufferOffset++, count--)
				{
					buffer[bufferOffset] = block[withinBlock];
				}
			}
		}

		public int Count
		{
			get { return m_Length; }
		}


		public void Read(int readOffset, T[] buffer)
		{
			Read(readOffset, buffer, 0, buffer.Length);
		}

		#region IEnumerable<T> Members

		public IEnumerator<T> GetEnumerator()
		{
			int index = 0;
			foreach (var block in m_Blocks)
			{
				if (block == null)
					yield break;

				foreach (var item in block)
				{
					if (index >= m_Length)
						yield break;

					yield return item;
					index++;
				}
			}
		}

		#endregion

		#region IEnumerable Members

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		#endregion
	}
}
