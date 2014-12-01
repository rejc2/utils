using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rejc2.Utils
{
	public static class ArrayUtils
	{
		public static S[] MapArray<T, S>(this T[] array, Func<T, S> map)
		{
			if (array == null)
				return null;

			S[] output = new S[array.Length];
			for (int i = 0; i < array.Length; i++)
			{
				output[i] = map(array[i]);
			}

			return output;
		}

		public static T[] CloneOrEmpty<T>(T[] array)
		{
			if (array == null || array.Length == 0)
				return ArrayUtils<T>.EmptyArray;

			return (T[])array.Clone();
		}
	}

	public static class ArrayUtils<T>
	{
		public static readonly T[] EmptyArray = new T[0];
	}
}
