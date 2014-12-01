using System;
using System.Collections.Generic;
using System.Text;

namespace Rejc2.Utils
{
	public interface ITrie<T> : ITrie2<string, T>
	{
	}

	public interface ITrie2<S, T>
	{
		S Path { get; }

		void Add(S subpath, T item);

		IList<T> GetItems(S subpath);

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
		IEnumerable<T> GetPossibleItems(S search, int index);

	}
}
