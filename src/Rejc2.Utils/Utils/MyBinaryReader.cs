using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Rejc2.Utils
{
	public class MyBinaryReader : BinaryReader
	{
		public MyBinaryReader(Stream stream)
			: base(stream, Encoding.UTF8)
		{
		}

		public new int Read7BitEncodedInt()
		{
			return base.Read7BitEncodedInt();
		}

		private readonly byte[] m_Buffer = new byte[8];

		public int ReadUInt24()
		{
			if (Read(m_Buffer, 0, 3) != 3)
				throw new EndOfStreamException();

			return m_Buffer[0] | m_Buffer[1] << 8 | m_Buffer[2] << 16;
		}

		public Character ReadCharacter()
		{
			char first = ReadUTF16();
			if (char.IsHighSurrogate(first))
				return new Character(first, ReadUTF16());
			else
				return new Character(first);
		}

		public char ReadUTF16()
		{
			return (char)ReadUInt16();
		}

		public List<T> ReadList<T>(Func<MyBinaryReader, T> read)
		{
			int count = Read7BitEncodedInt();
			var list = new List<T>(count);
			for (int i = 0; i < count; i++)
			{
				var item = read(this);
				list.Add(item);
			}
			return list;
		}

		public T[] ReadArray<T>(Func<MyBinaryReader, T> read)
		{
			int count = Read7BitEncodedInt();
			var list = new T[count];
			for (int i = 0; i < count; i++)
			{
				var item = read(this);
				list[i] = item;
			}
			return list;
		}

		public Dictionary<TKey, TValue> ReadDictionary<TKey, TValue>(
					Func<MyBinaryReader, TKey> readKey,
					Func<MyBinaryReader, TValue> readValue)
		{
			int count = Read7BitEncodedInt();
			var dictionary = new Dictionary<TKey, TValue>(count);
			for (int i = 0; i < count; i++)
			{
				var key = readKey(this);
				var value = readValue(this);
				dictionary.Add(key, value);
			}
			return dictionary;
		}
	}
}
