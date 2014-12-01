using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rejc2.Utils
{
	public static class Pair
	{
		public static Pair<TFirst, TSecond> New<TFirst, TSecond>(TFirst first, TSecond second)
		{
			return new Pair<TFirst, TSecond>(first, second);
		}
	}

	public struct Pair<TFirst,TSecond> : IEquatable<Pair<TFirst,TSecond>>
	{
		private static readonly Comparer DefaultComparer = new Comparer(null, null);

		private readonly TFirst _first;
		private readonly TSecond _second;

		public TFirst First
		{
			get { return _first; }
		}

		public TSecond Second
		{
			get { return _second; }
		}

		public Pair(TFirst first, TSecond second)
		{
			_first = first;
			_second = second;
		}

		#region IEquatable<Pair<TFirst,TSecond>> Members

		public bool Equals(Pair<TFirst, TSecond> other)
		{
			return DefaultComparer.Equals(this, other);
		}

		#endregion

		public override bool Equals(object obj)
		{
			if (!(obj is Pair<TFirst, TSecond>))
				return false;

			return Equals((Pair<TFirst, TSecond>)obj);
		}

		public override int GetHashCode()
		{
			return DefaultComparer.GetHashCode(this);
		}

//		public static IEqualityComparer<Pair<TFirst, TSecond>> GetComparer(
//			IEqualityComparer<TFirst> firstComparer = null,
//			IEqualityComparer<TSecond> secondComparer = null)
		public static IEqualityComparer<Pair<TFirst, TSecond>> GetComparer(
																	IEqualityComparer<TFirst> firstComparer,
																	IEqualityComparer<TSecond> secondComparer)
		{
			return new Comparer(firstComparer, secondComparer);
		}

		private class Comparer : IEqualityComparer<Pair<TFirst, TSecond>>
		{
			private readonly IEqualityComparer<TFirst> _firstComparer;
			private readonly IEqualityComparer<TSecond> _secondComparer;

			public Comparer(IEqualityComparer<TFirst> firstComparer, IEqualityComparer<TSecond> secondComparer)
			{
				_firstComparer = firstComparer ?? EqualityComparer<TFirst>.Default;
				_secondComparer = secondComparer ?? EqualityComparer<TSecond>.Default;
			}


			#region IEqualityComparer<Pair<TFirst,TSecond>> Members

			public bool Equals(Pair<TFirst, TSecond> x, Pair<TFirst, TSecond> y)
			{
				return _firstComparer.Equals(x._first, y._first) &&
						_secondComparer.Equals(x._second, y._second);
			}

			public int GetHashCode(Pair<TFirst, TSecond> obj)
			{
				int firstHashCode = _firstComparer.GetHashCode(obj._first);
				int secondHashcode = _secondComparer.GetHashCode(obj._second);

				return firstHashCode ^ (secondHashcode << 19) ^ (secondHashcode >> 13);
			}

			#endregion
		}
	}
}
