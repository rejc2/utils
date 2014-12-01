using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rejc2.Utils
{
	public static class ListUtils
	{
		public static IEnumerable<T> NotNull<T>(this IEnumerable<T> list)
		{
			if (list == null)
				return ArrayUtils<T>.EmptyArray;
			else
				return list;
		}

	}
}
