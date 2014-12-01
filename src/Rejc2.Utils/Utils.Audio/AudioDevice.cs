using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rejc2.Utils.Audio
{
	public abstract class AudioDevice : IDisposable
	{
		private readonly AudioSource m_Source;

		protected AudioDevice(AudioSource source)
		{
			if (source == null)
				throw new ArgumentNullException("source");

			m_Source = source;
		}

		public AudioSource Source
		{
			get { return m_Source; }
		}

		protected void FillBuffer(FillBufferEventArgs fillBufferArgs)
		{
			Source.OnFillBuffer(fillBufferArgs);
		}

		public abstract void Start();

		protected virtual void Dispose(bool disposing)
		{
		}

		public void Dispose()
		{
			Dispose(true);
		}

		~AudioDevice()
		{
			Dispose(false);
		}
	}
}
