using System;
using System.Collections.Generic;
using System.Text;
using System.Globalization;
using System.IO;

namespace Rejc2.Utils
{
	[Serializable]
	public struct Character : IEquatable<Character>
	{
		private readonly int m_Unicode;

		public static readonly Character None = new Character();
		public static readonly Character ReplacementCharacter = new Character('\uFFFD');
		public static readonly Character NewLine = new Character('\n');

		public int Unicode
		{
			[System.Diagnostics.DebuggerStepThrough]
			get { return m_Unicode; }
		}

		public bool WouldBeSurrogates
		{
			get { return (m_Unicode > 0xFFFF); }
		}

		public Character(int unicode)
		{
			m_Unicode = unicode;
		}

		public Character(char c)
		{
			if (Char.IsSurrogate(c))
				throw new ArgumentOutOfRangeException("c", "UTF16 code may not be surrogate");

			m_Unicode = (int)c;
		}

		public Character(char high, char low)
		{
			if (!Char.IsSurrogatePair(high, low))
				throw new ArgumentOutOfRangeException("high, low", "Arguments are not surrogate pair");

			m_Unicode = Char.ConvertToUtf32(high, low);
		}

		public Character(string text, int index)
		{
			if (string.IsNullOrEmpty(text))
				throw new ArgumentNullException("text");
			if (index < 0 || index >= text.Length)
				throw new ArgumentOutOfRangeException("index", index, "index");

			if (Char.IsLowSurrogate(text[index]))
			{
				this = ReplacementCharacter;
			}
			else if (Char.IsHighSurrogate(text[index]))
			{
				if (index >= text.Length - 1 || !Char.IsLowSurrogate(text[index + 1]))
					this = ReplacementCharacter;
				else
					m_Unicode = Char.ConvertToUtf32(text[index], text[index + 1]);
			}
			else
			{
				m_Unicode = (int)text[index];
			}
		}

		public static Character ParseU(string s)
		{
			if (s == null) throw new ArgumentNullException("s");
			if (!s.StartsWith("U+", StringComparison.Ordinal) &&
					!s.StartsWith("U-", StringComparison.Ordinal))
				throw new ArgumentOutOfRangeException("s", "Must start with 'U+'");

			return new Character(Int32.Parse(s.Substring(2), NumberStyles.HexNumber));
		}

		public static IEnumerable<Character> ReadText(string text)
		{
			return ReadText(text, false);
		}

		public static IEnumerable<Character> ReadText(string text, bool simplifyNewlines)
		{
			if (text == null) throw new ArgumentNullException("text");

			StringReader reader = new StringReader(text);

			while (true)
			{
				int read = reader.Read();
				if (read < 0)
					break;

				char ch = (char)read;
				if (Char.IsHighSurrogate(ch))
				{
					int peek = reader.Peek();
					if (peek >= 0)
					{
						char peekch = (char)peek;
						if (Char.IsLowSurrogate(peekch))
						{
							reader.Read();
							yield return new Character(ch, peekch);
						}
						else
						{
							yield return ReplacementCharacter;
						}
					}
				}
				else if (Char.IsLowSurrogate(ch))
				{
					yield return ReplacementCharacter;
				}
				else if (simplifyNewlines && ch == '\r')
				{
					int peek = reader.Peek();
					if (peek >= 0 && (char)peek == '\n')
					{
						reader.Read();
					}
					yield return Character.NewLine;
				}
				else
				{
					yield return new Character(ch);
				}
			}
		}

		public static int MoveNext(string text, int index)
		{
			if (index < 0)
				return 0;
			if (index >= text.Length)
				return index;

			if (char.IsHighSurrogate(text[index]) && index < text.Length - 1 &&
					char.IsLowSurrogate(text[index + 1]))
				index += 2;
			else
				index++;

			return index;
		}
		public static int MoveBack(string text, int index)
		{
			if (index == 0)
				return -1;
			if (index < 0)
				return index;

			if (index > text.Length)
				index = text.Length;

			if (char.IsLowSurrogate(text[index - 1]) && index >= 2 &&
					char.IsHighSurrogate(text[index - 2]))
				index -= 2;
			else
				index--;

			return index;
		}


		#region IEquatable<Character> Members

		public bool Equals(Character other)
		{
			return m_Unicode == other.m_Unicode;
		}

		#endregion

		public override bool Equals(object obj)
		{
			if (!(obj is Character))
				return false;

			return Equals((Character)obj);
		}
		public override int GetHashCode()
		{
			return m_Unicode.GetHashCode();
		}

		public override string ToString()
		{
			return Char.ConvertFromUtf32(m_Unicode);
		}

		public static bool operator ==(Character x, Character y)
		{
			return x.Equals(y);
		}
		public static bool operator !=(Character x, Character y)
		{
			return !x.Equals(y);
		}
		public static bool operator ==(Character x, char y)
		{
			if (char.IsSurrogate(y))
				return false;

			return x.Equals(new Character(y));
		}
		public static bool operator !=(Character x, char y)
		{
			return !(x == y);
		}
	}
}
