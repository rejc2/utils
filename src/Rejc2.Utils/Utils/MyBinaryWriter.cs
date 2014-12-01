using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Rejc2.Utils
{
	//public delegate void Action<TArg1, TArg2>(TArg1 arg1, TArg2 arg2);
	//public delegate TResult Func<TArg1, TResult>(TArg1 arg1);
	//public delegate TResult Func<TArg1, TArg2, TResult>(TArg1 arg1, TArg2 arg2);

	public class MyBinaryWriter : BinaryWriter
	{
		public MyBinaryWriter(Stream stream)
			: base(stream, Encoding.UTF8)
		{
		}

		public new void Write7BitEncodedInt(int value)
		{
			base.Write7BitEncodedInt(value);
		}

		private readonly byte[] m_Buffer = new byte[8];

		public void WriteUInt24(int value)
		{
			if (value < 0 || value >= (1 << 24))
				throw new ArgumentOutOfRangeException("value", value, "value is not UInt24");

			m_Buffer[0] = (byte)(value & 255);
			m_Buffer[1] = (byte)((value >> 8) & 255);
			m_Buffer[2] = (byte)((value >> 16) & 255);
			Write(m_Buffer, 0, 3);
		}

		public void Write(Character ch)
		{
			string str = ch.ToString();
			if (str.Length == 1)
				WriteUTF16(str[0]);
			else if (str.Length == 2)
			{
				WriteUTF16(str[0]);
				WriteUTF16(str[1]);
			}
			else
				throw new InvalidOperationException();
		}

		public void WriteUTF16(char ch)
		{
			Write((ushort)ch);
		}

		public void WriteList<T>(ICollection<T> list, Action<MyBinaryWriter, T> write)
		{
			Write7BitEncodedInt(list.Count);
			foreach (T item in list)
			{
				write(this, item);
			}
		}

		public void WriteDictionary<TKey, TValue>(IDictionary<TKey, TValue> dictionary,
						Action<MyBinaryWriter, TKey> writeKey,
						Action<MyBinaryWriter, TValue> writeValue)
		{
			Write7BitEncodedInt(dictionary.Count);
			foreach (var item in dictionary)
			{
				writeKey(this, item.Key);
				writeValue(this, item.Value);
			}
		}
	}


}
