using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Diagnostics;

namespace Rejc2.Utils
{
	public class Trie<T> : ITrie<T>
	{
		public string Path { get; private set; }

		private readonly List<T> Items = new List<T>();

		private readonly HybridDictionary<char, Trie<T>> m_Subtries;

		public Trie()
			: this(string.Empty, 0)
		{
		}

		private Trie(string path, int capacity)
		{
			Path = path;
			m_Subtries = new HybridDictionary<char, Trie<T>>(capacity);
		}

		public void Add(string subpath, T item)
		{
			if (String.IsNullOrEmpty(subpath))
			{
				Items.Add(item);
				return;
			}

			Trie<T> subtrie;
			if (!m_Subtries.TryGetValue(subpath[0], out subtrie))
			{
				subtrie = new Trie<T>(Path + subpath[0], (subpath.Length > 1) ? 1 : 0);
				m_Subtries.Add(subpath[0], subtrie);
			}

			subtrie.Add(subpath.Substring(1), item);
		}

		private static readonly T[] s_EmptyItems = new T[0];

		/// <summary>
		/// Returns all the items exactly matching the string <paramref name="subpath"/>.
		/// </summary>
		/// <param name="subpath"></param>
		/// <returns></returns>
		public IList<T> GetItems(string subpath)
		{
			if (String.IsNullOrEmpty(subpath))
			{
				return Items.ToArray();
			}

			Trie<T> subtrie;
			if (!m_Subtries.TryGetValue(subpath[0], out subtrie))
			{
				return s_EmptyItems;
			}

			return subtrie.GetItems(subpath.Substring(1));

		}

		/// <summary>
		/// Returns all the items in the entire trie. The top-level items are returned
		/// first, followed by a depth-first iteration of the deeper levels.
		/// </summary>
		/// <returns></returns>
		public IEnumerable<T> GetAllItems()
		{
			foreach (T item in Items)
			{
				yield return item;
			}

			foreach (var subtrie in m_Subtries)
			{
				foreach (T item in subtrie.Value.GetAllItems())
				{
					yield return item;
				}
			}
		}

		/// <summary>
		/// Returns the items matching the string <paramref name="subpath"/>, as well
		/// as all possible longer strings. The exactly matching items are returned
		/// first, followed by a depth-first iteration of the deeper levels.
		/// </summary>
		/// <param name="subpath"></param>
		/// <returns></returns>
		/// <remarks>Used by the character stroke search feature.</remarks>
		public IEnumerable<T> GetAllItems(string subpath)
		{
			if (subpath.Length == 0)
			{
				return GetAllItems();
			}

			Trie<T> subtrie;
			if (!m_Subtries.TryGetValue(subpath[0], out subtrie))
			{
				return s_EmptyItems;
			}

			return subtrie.GetAllItems(subpath.Substring(1));
		}

		/// <summary>
		/// Returns all possible matches of substrings of <paramref name="search"/>
		/// starting at <paramref name="index"/>.
		/// Longest matching substrings are returned first, followed by shorter
		/// matching substrings.
		/// </summary>
		/// <remarks>Used by the annotation and dictionary features.</remarks>
		/// <param name="search"></param>
		/// <param name="index"></param>
		/// <returns></returns>
		public IEnumerable<T> GetPossibleItems(string search, int index)
		{
			if (index < search.Length)
			{

				Trie<T> subtrie;
				if (m_Subtries.TryGetValue(search[index], out subtrie))
				{
					foreach (T item in subtrie.GetPossibleItems(search, index + 1))
					{
						yield return item;
					}
				}
			}

			foreach (T item in Items)
				yield return item;
		}

		public void WriteToFile(MyBinaryWriter writer, Action<MyBinaryWriter, T> writeData)
		{
			writer.Write7BitEncodedInt(m_Subtries.Count);
			foreach (KeyValuePair<char, Trie<T>> subtrie in m_Subtries)
			{
				writer.WriteUTF16(subtrie.Key);
				subtrie.Value.WriteToFile(writer, writeData);
			}
			
			writer.Write7BitEncodedInt(Items.Count);
			foreach (T item in Items)
			{
				writeData(writer, item);
			}
		}

		public static Trie<T> ReadFile(MyBinaryReader reader, Func<MyBinaryReader, T> readData)
		{
			return ReadFile(string.Empty, reader, readData);
		}

		private static Trie<T> ReadFile(string path, MyBinaryReader reader, Func<MyBinaryReader, T> readData)
		{
			int subTries = reader.Read7BitEncodedInt();
			Trie<T> trie = new Trie<T>(path, subTries);

			for (int i = 0; i < subTries; i++)
			{
				char ch = reader.ReadUTF16();
				trie.m_Subtries.Add(ch, ReadFile(path + ch, reader, readData));
			}

			int items = reader.Read7BitEncodedInt();
			trie.Items.Capacity = items;
			for (int i = 0; i < items; i++)
			{
				trie.Items.Add(readData(reader));
			}

			return trie;
		}

		/// <summary>
		/// Used by WriteToMiniFile to see if this trie is suitable.
		/// </summary>
		/// <param name="usedChars"></param>
		private void GetUsedChars(Set<char> usedChars)
		{
			foreach (var charSubtrie in m_Subtries)
			{
				usedChars.Add(charSubtrie.Key);
				charSubtrie.Value.GetUsedChars(usedChars);
			}
		}

		/// <summary>
		/// Where 8 or fewer different characters are used to make up the strings,
		/// we can write a more efficient kind of file.
		/// Falls back to normal file if there are more than 8 different
		/// characters used.
		/// </summary>
		/// <param name="writer"></param>
		/// <param name="writeData"></param>
		/// <remarks>Used for the stroke finder</remarks>
		public void WriteToMiniFile(MyBinaryWriter writer, Action<MyBinaryWriter, T> writeData)
		{
			var usedChars = new Set<char>();
			GetUsedChars(usedChars);
			if (usedChars.Count > 8)
			{
				writer.Write(string.Empty);
				WriteToFile(writer, writeData);
				return;
			}
			StringBuilder charIndexBuilder = new StringBuilder(usedChars.Count);
			foreach (char ch in usedChars)
			{
				charIndexBuilder.Append(ch);
			}
			string charIndex = charIndexBuilder.ToString();

			writer.Write(charIndex);

			WriteToMiniFile(charIndex, writer, writeData);
		}

		private void WriteToMiniFile(string charIndex, MyBinaryWriter writer, Action<MyBinaryWriter, T> writeData)
		{
			Debug.Assert(charIndex.Length <= 8);

			byte subTries = 0;
			for (int i = 0; i<charIndex.Length; i++)
			{
				if (m_Subtries.ContainsKey(charIndex[i]))
					subTries |= (byte)(1 << i);
			}

			writer.Write(subTries);

			//writer.Write7BitEncodedInt(m_Subtries.Count);
			//foreach (KeyValuePair<char, Trie<T>> subtrie in m_Subtries)
			for (int i = 0; i < charIndex.Length; i++)
			{
				if ((subTries & (1 << i)) == 0)
					continue;

				var subtrie = m_Subtries[charIndex[i]];

				//writer.WriteUTF16(subtrie.Key);
				subtrie.WriteToMiniFile(charIndex, writer, writeData);
			}
			
			writer.Write7BitEncodedInt(Items.Count);
			foreach (T item in Items)
			{
				writeData(writer, item);
			}
		}

		public static Trie<T> ReadMiniFile(MyBinaryReader reader, Func<MyBinaryReader, T> readData)
		{
			string charIndex = reader.ReadString();
			if (charIndex.Length == 0)
				return ReadFile(reader, readData);

			if (charIndex.Length > 8)
				throw new InvalidOperationException("charIndex too long: " + charIndex);

			int[] counts = new int[1 << charIndex.Length];
			for (int i = 0; i < counts.Length; i++)
			{
				int count = 0;
				for (int j = 0; j < charIndex.Length; j++)
				{
					if ((i & (1 << j)) != 0)
						count++;
				}
				counts[i] = count;
			}

			return ReadMiniFile(charIndex, counts, string.Empty, reader, readData);
		}

		private static Trie<T> ReadMiniFile(string charIndex, int[] counts, string path, MyBinaryReader reader, Func<MyBinaryReader, T> readData)
		{
			//int subTries = reader.Read7BitEncodedInt();
			byte subTries = reader.ReadByte();
			Trie<T> trie = new Trie<T>(path, counts[subTries]);

			for (int i = 0; i < charIndex.Length; i++)
			{
				if ((subTries & (1 << i)) == 0)
					continue;

				char ch = charIndex[i];
				trie.m_Subtries.Add(ch, ReadMiniFile(charIndex, counts, path + ch, reader, readData));
			}

			int items = reader.Read7BitEncodedInt();
			trie.Items.Capacity = items;
			for (int i = 0; i < items; i++)
			{
				trie.Items.Add(readData(reader));
			}

			return trie;
		}



	}
}
