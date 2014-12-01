using System;
using System.Collections.Generic;
using System.Text;

namespace Rejc2.Utils
{
	[Serializable]
	public class SetDictionary<S, T> : IEnumerable<KeyValuePair<S,T>>
	{
		private readonly IEqualityComparer<T> m_SetComparer;
		private readonly Dictionary<S, Set<T>> m_Dictionary;

		private static readonly T[] s_EmptyValues = new T[0];

		public SetDictionary()
			: this(null, null)
		{ }
		public SetDictionary(IEqualityComparer<S> comparer)
			: this(comparer, null)
		{ }
		public SetDictionary(IEqualityComparer<S> comparer, IEqualityComparer<T> setComparer)
		{
			m_SetComparer = setComparer;
			m_Dictionary = new Dictionary<S, Set<T>>(comparer);
		}

		public void Add(S key, T value)
		{
			Set<T> set;
			if (!m_Dictionary.TryGetValue(key, out set))
			{
				set = new Set<T>(m_SetComparer);
				m_Dictionary.Add(key, set);
			}

			set.Add(value);
		}

		public void AddRange(IEnumerable<KeyValuePair<S, T>> keysValues)
		{
			foreach (var keyValue in keysValues)
			{
				Add(keyValue.Key, keyValue.Value);
			}
		}

		public void AddRange(S key, IEnumerable<T> values)
		{
			Set<T> set;
			if (!m_Dictionary.TryGetValue(key, out set))
			{
				set = new Set<T>(m_SetComparer);
				m_Dictionary.Add(key, set);
			}

			foreach (var value in values)
			{
				set.Add(value);
			}
		}

		public ICollection<T> this[S key]
		{
			get
			{
				if (!m_Dictionary.ContainsKey(key))
					return s_EmptyValues;

				return m_Dictionary[key];
			}
		}

		public bool ContainsAny(S key)
		{
			return m_Dictionary.ContainsKey(key) && m_Dictionary[key].Count > 0;
		}

		public bool Contains(S key, T value)
		{
			if (!m_Dictionary.ContainsKey(key))
				return false;

			return m_Dictionary[key].Contains(value);
		}

		public void AddOrRemove(S key, T value, bool add)
		{
			if (add)
				Add(key, value);
			else
				Remove(key, value);
		}

		public IEnumerable<S> Keys
		{
			get
			{
				foreach (KeyValuePair<S, Set<T>> keySet in m_Dictionary)
				{
					if (keySet.Value.Count > 0)
						yield return keySet.Key;
				}
			}
		}

		public IEnumerable<KeyValuePair<S, Set<T>>> KeysSets
		{
			get
			{
				foreach (KeyValuePair<S, Set<T>> keySet in m_Dictionary)
				{
					if (keySet.Value.Count > 0)
						yield return keySet;
				}
			}
		}

		public IEnumerable<T> Values
		{
			get
			{
				foreach (Set<T> set in m_Dictionary.Values)
				{
					foreach (T item in set)
					{
						yield return item;
					}
				}
			}
		}

		public void Remove(S key, T value)
		{
			if (!m_Dictionary.ContainsKey(key))
				return;

			m_Dictionary[key].Remove(value);
		}

		public void RemoveAll(S key)
		{
			if (!m_Dictionary.ContainsKey(key))
				return;

			m_Dictionary.Remove(key);
		}

		public void RemoveEverywhere(T value)
		{
			foreach (S key in m_Dictionary.Keys)
			{
				Remove(key, value);
			}
		}


		#region IEnumerable<KeyValuePair<S,T>> Members

		public IEnumerator<KeyValuePair<S, T>> GetEnumerator()
		{
			foreach (KeyValuePair<S, Set<T>> keyValue in m_Dictionary)
			{
				foreach (T value in keyValue.Value)
				{
					yield return new KeyValuePair<S, T>(keyValue.Key, value);
				}
			}
		}

		#endregion

		#region IEnumerable Members

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		#endregion
	}
}
