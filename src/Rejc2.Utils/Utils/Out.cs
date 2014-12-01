using System;
using System.Collections.Generic;
using System.Text;

namespace Rejc2.Utils
{
	public static class Out
	{
		public static Out<T> New<T>(T value)
		{
			return new Out<T>(value);
		}
	}

	public class Out<T>
	{
		public Out()
		{
		}

		public Out(T value)
		{
			Value = value;
		}

		public T Value { get; set; }
	}
}
