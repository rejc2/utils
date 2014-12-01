using System;
using System.Collections.Generic;
using System.Text;

namespace Rejc2.Utils
{
	public class HybridDictionary<TKey, TValue> : IEnumerable<KeyValuePair<TKey, TValue>>
	{
		private const int Threshold = 7;

		private static readonly IEqualityComparer<TKey> Comparer = EqualityComparer<TKey>.Default;

		private Dictionary<TKey, TValue> m_Dictionary;
		private KeyValuePair<TKey, TValue>[] m_List;
		private int m_Count;

		public HybridDictionary()
			: this(0)
		{
		}

		public HybridDictionary(int capacity)
		{
			m_Dictionary = null;
			m_List = null;
			m_Count = 0;

			Capacity = capacity;
		}

		public int Capacity
		{
			get
			{
				if (m_Dictionary != null)
					return Math.Max(m_Dictionary.Count, Threshold);
				else if (m_List != null)
					return m_List.Length;
				else
					return 0;
			}
			set
			{
				if (m_Dictionary == null)
				{
					if (value >= Threshold)
					{
						m_Dictionary = new Dictionary<TKey, TValue>(value);
						if (m_List != null)
						{
							foreach (var keyValue in m_List)
							{
								m_Dictionary.Add(keyValue.Key, keyValue.Value);
							}

							m_List = null;
						}
					}
					else if (value > 0)
					{
						if (m_List == null)
							m_List = new KeyValuePair<TKey, TValue>[value];
						else if (value > m_List.Length)
						{
							var newList = new KeyValuePair<TKey, TValue>[value];
							Array.Copy(m_List, newList, m_List.Length);
							m_List = newList;
						}
					}
				}
			}
		}

		public int Count
		{
			get
			{
				if (m_Dictionary != null)
					return m_Dictionary.Count;
				else
					return m_Count;
			}
		}

		public void Add(TKey key, TValue value)
		{
			if (m_Dictionary == null)
			{
				if (m_List == null || m_List.Length <= m_Count)
				{
					Capacity = m_Count + 1;
				}
			}

			if (m_Dictionary != null)
			{
				m_Dictionary.Add(key, value);
			}
			else
			{
				m_List[m_Count] = new KeyValuePair<TKey, TValue>(key, value);
				m_Count++;
			}
		}

		public bool TryGetValue(TKey key, out TValue value)
		{
			if (m_Dictionary != null)
			{
				return m_Dictionary.TryGetValue(key, out value);
			}
			else
			{
				for (int i = 0; i < m_Count; i++)
				{
					if (Comparer.Equals(m_List[i].Key, key))
					{
						value = m_List[i].Value;
						return true;
					}
				}
				value = default(TValue);
				return false;
			}
		}

		public bool ContainsKey(TKey key)
		{
			TValue value;
			return TryGetValue(key, out value);
		}

		public TValue this[TKey key]
		{
			get
			{
				TValue value;
				if (!TryGetValue(key, out value))
					throw new KeyNotFoundException();

				return value;
			}
			set
			{
				throw new NotSupportedException();
			}
		}

		#region IEnumerable<KeyValuePair<TKey,TValue>> Members

		public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
		{
			if (m_Dictionary != null)
				return m_Dictionary.GetEnumerator();
			else
				return GetListEnumerator();				
		}

		private IEnumerator<KeyValuePair<TKey, TValue>> GetListEnumerator()
		{
			for (int i = 0; i < m_Count; i++)
			{
				yield return m_List[i];
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
