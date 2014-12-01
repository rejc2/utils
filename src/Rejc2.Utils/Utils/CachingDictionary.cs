using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rejc2.Utils
{
	public class CachingDictionary<TKey, TValue>
	{
		private volatile Dictionary<TKey, TValue> stableDictionary;

		private readonly IEqualityComparer<TKey> m_Comparer;
		private readonly Func<TKey, TValue> m_Func;

		private readonly object m_Lock = new object();
		private readonly Dictionary<TKey, TValue> updatableDictionary;
		private int m_GetCount;

		public CachingDictionary(Func<TKey, TValue> function)
		{
			m_Comparer = EqualityComparer<TKey>.Default;
			m_Func = function;

			stableDictionary = new Dictionary<TKey, TValue>(m_Comparer);
			updatableDictionary = new Dictionary<TKey,TValue>(m_Comparer);
			m_GetCount = 0;
		}

		public TValue this[TKey key]
		{
			get { return GetValue(key); }
		}

		public TValue GetValue(TKey key)
		{
			var currentDictionary = stableDictionary;
			TValue value;
			if (currentDictionary.TryGetValue(key, out value))
				return value;

			lock (m_Lock)
			{
				if (currentDictionary != stableDictionary)
				{
					if (stableDictionary.TryGetValue(key, out value))
						return value;
				}

				if (updatableDictionary.TryGetValue(key, out value))
				{
					m_GetCount++;
					return value;
				}

				value = m_Func(key);

				updatableDictionary.Add(key, value);
				m_GetCount++;

				if (m_GetCount > stableDictionary.Count * 4)
				{
					var newDictionary = new Dictionary<TKey, TValue>(stableDictionary, m_Comparer);
					foreach (var item in updatableDictionary)
					{
						newDictionary.Add(item.Key, item.Value);
					}

					stableDictionary = newDictionary;

					updatableDictionary.Clear();
					m_GetCount = 0;
				}

				return value;
			}
		}


	}
}
