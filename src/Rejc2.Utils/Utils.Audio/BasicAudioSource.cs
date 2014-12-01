using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rejc2.Utils.Audio
{
	public class BasicAudioSource : AudioSource
	{
		public BasicAudioSource(float sampleRate)
			: base(sampleRate)
		{
		}

		private class NowPlaying
		{
			public Wavefile wavefile;
			public volatile int through;
		}

		private readonly object m_lock = new object();
		//private readonly List<NowPlaying> nowPlaying = new List<NowPlaying>();
		private SimpleLinkedList<NowPlaying> nowPlaying = SimpleLinkedList<NowPlaying>.Empty;

		public void PlayWavefile(Wavefile wavefile)
		{
			PlayWavefile(wavefile, TimeSpan.Zero);
		}

		public void PlayWavefile(Wavefile wavefile, TimeSpan delay)
		{
			if (wavefile == null)
				return;

			var playing = new NowPlaying
			{
				wavefile = wavefile,
				through = -(int)(delay.TotalSeconds * SampleRate)
			};

			lock (m_lock)
			{
				nowPlaying = nowPlaying.AddHead(playing);
			}
		}

		private Sample[] spareBuffer;

		protected internal override void OnFillBuffer(FillBufferEventArgs e)
		{
			base.OnFillBuffer(e);

			if (spareBuffer == null || spareBuffer.Length < e.Buffer.Length)
			{
				spareBuffer = new Sample[e.Buffer.Length];
			}

			bool needToRemove = false;
			foreach (var playing in nowPlaying)
			{
				playing.wavefile.GetSampleData(playing.through, spareBuffer, 0, e.Buffer.Length);
				playing.through += e.Buffer.Length;

				if (playing.through >= playing.wavefile.NumberOfSamples)
					needToRemove = true;

				for (int i = 0; i < e.Buffer.Length; i++)
				{
					e.Buffer[i] += spareBuffer[i];
				}
			}

			if (needToRemove)
			{
				lock (nowPlaying)
				{
					//nowPlaying.RemoveAll(p => p.through >= p.wavefile.NumberOfSamples);
					nowPlaying = SimpleLinkedList<NowPlaying>.CreateReversed(
							nowPlaying.Where(p => p.through < p.wavefile.NumberOfSamples));
				}
			}

			//for (int i=0; i<
		}

	}
}
