using System;
using System.Collections.Generic;
using System.Text;

namespace Rejc2.Utils
{
	[Serializable]
	public class Set<T> : IEnumerable<T>, ICollection<T>
	{
		[Serializable]
		[System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential, Size=0)]
		struct MyVoid {
			public override string ToString()
			{
				return String.Empty;
			}
			public override bool Equals(object obj)
			{
				return true;
			}
			public override int GetHashCode()
			{
				return 0;
			}
		}

		private readonly Dictionary<T, MyVoid> m_Dictionary;

		public Set()
			: this((IEqualityComparer<T>)null)
		{ }
		public Set(IEqualityComparer<T> comparer)
		{
			m_Dictionary = new Dictionary<T, MyVoid>(comparer);
		}

		public Set(params T[] contents)
			: this(null, contents)
		{
		}

		public Set(IEqualityComparer<T> comparer, params T[] contents)
			: this(comparer)
		{
			foreach (T item in contents)
			{
				Add(item);
			}
		}

		public IEqualityComparer<T> Comparer
		{
			get { return m_Dictionary.Comparer; }
		}

		public void Add(T value)
		{
			if (!m_Dictionary.ContainsKey(value))
				m_Dictionary.Add(value, default(MyVoid));
		}

		public void AddRange(IEnumerable<T> values)
		{
			foreach (T value in values)
			{
				Add(value);
			}
		}

		public bool Remove(T value)
		{
			return m_Dictionary.Remove(value);
		}

		public void RemoveRange(IEnumerable<T> values)
		{
			foreach (T value in values)
			{
				Remove(value);
			}
		}

		public int Count
		{
			get { return m_Dictionary.Count; }
		}


		#region IEnumerable<T> Members

		public IEnumerator<T> GetEnumerator()
		{
			return m_Dictionary.Keys.GetEnumerator();
		}

		#endregion

		#region IEnumerable Members

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return m_Dictionary.Keys.GetEnumerator();
		}

		#endregion

		public bool Contains(T value)
		{
			return m_Dictionary.ContainsKey(value);
		}

		public static Set<T> operator &(Set<T> first, Set<T> second)
		{
			Set<T> ret = new Set<T>();
			foreach (T item in first)
			{
				if (second.Contains(item)) ret.Add(item);
			}

			return ret;
		}

		#region ICollection<T> Members

		public void Clear()
		{
			m_Dictionary.Clear();
		}

		public void CopyTo(T[] array, int arrayIndex)
		{
			m_Dictionary.Keys.CopyTo(array, arrayIndex);
		}

		bool ICollection<T>.IsReadOnly
		{
			get { return false; }
		}

		#endregion
	}
}
