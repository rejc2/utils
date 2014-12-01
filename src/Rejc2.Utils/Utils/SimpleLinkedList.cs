using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rejc2.Utils
{
	public class SimpleLinkedList<T> : ICollection<T>
	{
		public readonly int Count;
		private T m_value;
		private SimpleLinkedList<T> m_next;

		public static readonly SimpleLinkedList<T> Empty = new SimpleLinkedList<T>();

		public SimpleLinkedList()
		{
		}

		public SimpleLinkedList(T value, SimpleLinkedList<T> next)
		{
			if (next == null) throw new ArgumentNullException("next");

			m_value = value;
			m_next = next;
			Count = m_next.Count + 1;
		}

		public static SimpleLinkedList<T> CreateReversed(IEnumerable<T> items)
		{
			var list = SimpleLinkedList<T>.Empty;

			foreach (var item in items)
			{
				list = list.AddHead(item);
			}

			return list;
		}

		public T Value
		{
			get
			{
				if (Count == 0)
					throw new InvalidOperationException();

				return m_value;
			}
		}

		public SimpleLinkedList<T> Next
		{
			get
			{
				return m_next;
			}
		}

		public SimpleLinkedList<T> AddHead(T item)
		{
			return new SimpleLinkedList<T>(item, this);
		}


		#region ICollection<T> Members

		void ICollection<T>.Add(T item)
		{
			throw new NotSupportedException();
		}

		void ICollection<T>.Clear()
		{
			throw new NotSupportedException();
		}

		public bool Contains(T item)
		{
			throw new NotImplementedException();
		}

		public void CopyTo(T[] array, int arrayIndex)
		{
			throw new NotImplementedException();
		}

		int ICollection<T>.Count
		{
			get { return Count; }
		}

		public bool IsReadOnly
		{
			get { return true; }
		}

		bool ICollection<T>.Remove(T item)
		{
			throw new NotSupportedException();
		}

		#endregion

		#region IEnumerable<T> Members

		public IEnumerator<T> GetEnumerator()
		{
			for (SimpleLinkedList<T> list = this; list.Count > 0; list = list.Next)
			{
				yield return list.Value;
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
