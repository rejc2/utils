using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rejc2.Utils
{
	public static class StringUtils
	{
		public static string FromTo(this string text, string from, string to)
		{
			var comparisonType = StringComparison.Ordinal;

			int fromIndex = from != null ? text.IndexOf(from, comparisonType) + from.Length : 0;
			if (fromIndex < 0)
				fromIndex = text.Length;

			int toIndex = to != null ? text.IndexOf(to, fromIndex, comparisonType) : text.Length;
			if (toIndex < 0)
				toIndex = text.Length;

			return text.Substring(fromIndex, toIndex - fromIndex);
		}

		public static int? TryParseInt32(this string text)
		{
			if (text == null) throw new ArgumentNullException("text");

			int i;
			if (int.TryParse(text, out i))
				return i;
			else
				return null;
		}

		public static IEnumerable<string> Sections(string text, string begin, string end)
		{
			if (text == null) throw new ArgumentNullException("text");
			if (begin == null) throw new ArgumentNullException("begin");
			if (end == null) throw new ArgumentNullException("end");
			if (begin.Length == 0)
				throw new ArgumentOutOfRangeException("begin", begin, " ");
			if (end.Length == 0)
				throw new ArgumentOutOfRangeException("end", end, " ");

			int current = 0;
			while (current < text.Length)
			{
				int next = text.IndexOf(begin, current, StringComparison.Ordinal);
				if (next < 0) next = text.Length;

				if (next > current)
				{
					yield return text.Substring(current, next - current);
					current = next;
				}

				if (next < text.Length)
				{
					int nextEnd = text.IndexOf(end, next + begin.Length, StringComparison.Ordinal);
					if (nextEnd < 0)
					{
						nextEnd = text.Length;
					}
					else
					{
						nextEnd += end.Length;
					}

					yield return text.Substring(next, nextEnd - next);
					current = nextEnd;
				}
			}
		}
	}
}
