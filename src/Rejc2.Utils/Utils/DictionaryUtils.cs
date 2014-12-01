using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rejc2.Utils
{
	public static class DictionaryUtils
	{
		public static TValue GetOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> dictionary,
						TKey key)
		{
			return dictionary.GetOrDefault(key, default(TValue));
		}

		public static TValue GetOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> dictionary,
								TKey key, TValue defaultValue)
		{
			TValue value;
			if (dictionary.TryGetValue(key, out value))
				return value;
			else
				return defaultValue;
		}
	}
}
