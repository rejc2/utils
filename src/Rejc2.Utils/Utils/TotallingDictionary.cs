using System;
using System.Collections.Generic;
using System.Text;

namespace Rejc2.Utils
{
	public class TotallingDictionary<TKey, TValue> : IDictionary<TKey, TValue>
	{
		public delegate TValue Combiner(TValue existing, TValue adding);

		private readonly Dictionary<TKey, TValue> m_Dictionary;
		private readonly TValue m_InitialValue;
		private readonly Combiner m_Summation;

		public TotallingDictionary(TValue initialValue, Combiner summation)
			: this(null, initialValue, summation)
		{
		}

		public TotallingDictionary(Combiner summation)
			: this(null, default(TValue), summation)
		{
		}

		public TotallingDictionary(IEqualityComparer<TKey> comparer,
				TValue initialValue, Combiner summation)
		{
			m_Dictionary = new Dictionary<TKey, TValue>(comparer);
			m_InitialValue = initialValue;
			m_Summation = summation;
		}

		#region IDictionary<TKey,TValue> Members

		public void Add(TKey key, TValue value)
		{
			TValue oldValue;
			if (!m_Dictionary.TryGetValue(key, out oldValue))
			{
				oldValue = m_InitialValue;
			}
			m_Dictionary[key] = m_Summation(oldValue, value);
		}

		public bool ContainsKey(TKey key)
		{
			return m_Dictionary.ContainsKey(key);
		}

		public ICollection<TKey> Keys
		{
			get { return m_Dictionary.Keys; }
		}

		public bool Remove(TKey key)
		{
			return m_Dictionary.Remove(key);
		}

		public bool TryGetValue(TKey key, out TValue value)
		{
			bool found = m_Dictionary.TryGetValue(key, out value);
			if (!found)
				value = m_InitialValue;

			return found;
		}

		public ICollection<TValue> Values
		{
			get { return m_Dictionary.Values; }
		}

		public TValue this[TKey key]
		{
			get
			{
				TValue value;
				TryGetValue(key, out value);
				return value;
			}
			set
			{
				m_Dictionary[key] = value;
			}
		}

		#endregion

		#region ICollection<KeyValuePair<TKey,TValue>> Members

		void ICollection<KeyValuePair<TKey,TValue>>.Add(KeyValuePair<TKey, TValue> item)
		{
			Add(item.Key, item.Value);
		}

		public void Clear()
		{
			m_Dictionary.Clear();
		}

		bool ICollection<KeyValuePair<TKey, TValue>>.Contains(KeyValuePair<TKey, TValue> item)
		{
			throw new NotImplementedException();
		}

		void ICollection<KeyValuePair<TKey, TValue>>.CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
		{
			throw new NotImplementedException();
		}

		public int Count
		{
			get { return m_Dictionary.Count; }
		}

		public bool IsReadOnly
		{
			get { return false; }
		}

		bool ICollection<KeyValuePair<TKey, TValue>>.Remove(KeyValuePair<TKey, TValue> item)
		{
			throw new NotImplementedException();
		}

		#endregion

		#region IEnumerable<KeyValuePair<TKey,TValue>> Members

		public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
		{
			return m_Dictionary.GetEnumerator();
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
