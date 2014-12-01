using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Rejc2.Utils
{
	public sealed class QueueAdandonedException : ApplicationException
	{
	}

	public interface QueueReader<T>
	{
		bool Closed { get;}
		bool Abandoned { get;}
		event EventHandler ItemsAdded;
		event EventHandler ClosedEvent;
		void Abandon();
		T[] DequeueAll(bool wait);
	}

	public interface QueueWriter<T>
	{
		bool Closed { get;}
		bool Abandoned { get;}
		void Enqueue(T item);
		void Enqueue(params T[] items);
		void Close();
	}

	class ThreadedQueue<T> : QueueReader<T>, QueueWriter<T>
	{
		private readonly Queue<T> m_Queue;

		private bool m_Closed;
		public bool Closed
		{
			get { lock (m_Queue) { return m_Closed; } }
		}

		private bool m_Abandoned;
		public bool Abandoned
		{
			get { lock (m_Queue) { return m_Abandoned; } }
		}

		public ThreadedQueue()
		{
			m_Queue = new Queue<T>();
		}

		public event EventHandler ItemsAdded;

		protected virtual void OnItemsAdded()
		{
			if (ItemsAdded != null)
			{
				ItemsAdded(this, new EventArgs());
			}
		}

		public event EventHandler ClosedEvent;

		protected virtual void OnClosed()
		{
			if (ClosedEvent != null)
			{
				ClosedEvent(this, new EventArgs());
			}
		}

		public void Enqueue(T item)
		{
			bool raiseEvent;

			lock (m_Queue)
			{
				if (m_Abandoned)
					throw new QueueAdandonedException();

				if (m_Closed)
					throw new InvalidOperationException();

				raiseEvent = (m_Queue.Count == 0);

				m_Queue.Enqueue(item);
				Monitor.PulseAll(m_Queue);
			}

			// Only raise event after unlocking queue
			if (raiseEvent)
				OnItemsAdded();
		}

		public void Enqueue(params T[] items)
		{
			bool raiseEvent;

			lock (m_Queue)
			{
				if (m_Abandoned)
					throw new QueueAdandonedException();

				if (m_Closed)
					throw new InvalidOperationException();

				raiseEvent = (m_Queue.Count == 0);

				foreach (T item in items)
					m_Queue.Enqueue(item);
				Monitor.PulseAll(m_Queue);
			}

			// Only raise event after unlocking queue
			if (raiseEvent)
				OnItemsAdded();
		}

		public void Close()
		{
			lock (m_Queue)
			{
				if (m_Abandoned)
					throw new QueueAdandonedException();

				if (m_Closed)
					return;

				m_Closed = true;
			}

			OnClosed();

			// These will never be called again in any case:
			ItemsAdded = null;
			ClosedEvent = null;
		}

		public void Abandon()
		{
			lock (m_Queue)
			{
				if (m_Abandoned)
					return;

				m_Abandoned = true;
			}

			// These will never be called again in any case:
			ItemsAdded = null;
			ClosedEvent = null;
		}

		public T[] DequeueAll()
		{
			return DequeueAll(false);
		}

		public T[] DequeueAll(bool wait)
		{
			T[] array;

			lock (m_Queue)
			{
				if (wait)
				{
					while (m_Queue.Count == 0 && !m_Closed)
					{
						Monitor.Wait(m_Queue);
					}
				}

				array = m_Queue.ToArray();
				m_Queue.Clear();
			}

			return array;
		}
	}
}
