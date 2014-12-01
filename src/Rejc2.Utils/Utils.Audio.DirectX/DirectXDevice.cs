using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Rejc2.Utils.Audio;
using SlimDX.Multimedia;
using SlimDX.DirectSound;
using DS = SlimDX.DirectSound;

namespace Rejc2.Utils.Audio.DirectX
{
	public class DirectXDevice : AudioDevice
	{
		readonly DS.DirectSound m_SoundDevice;

		SoundBuffer m_Buffer;

		public DirectXDevice(AudioSource audio, IntPtr owner)
			: base(audio)
		{
			m_SoundDevice = new DS.DirectSound();
			m_SoundDevice.SetCooperativeLevel(owner, CooperativeLevel.Normal);
		}

		bool m_Started = false;

		int m_SampleRate;
		int m_Channels;
		int m_BytesPerSample;
		int m_ChunkSize; // in samples
		int m_ChunkSizeBytes; // in bytes
		int m_NumChunks;
		volatile bool m_Abort;

		Thread m_PlayingThread;

		public override void Start()
		{
			if (m_Started)
				return;

			WaveFormat waveformat = new WaveFormat();

			m_SampleRate = (int)Source.SampleRate;

			waveformat.FormatTag = WaveFormatTag.Pcm;
			waveformat.BitsPerSample = 16;
			waveformat.Channels = 2;
			waveformat.SamplesPerSecond = m_SampleRate;
			waveformat.BlockAlignment = (short)(waveformat.Channels * (waveformat.BitsPerSample / 8));
			waveformat.AverageBytesPerSecond = waveformat.SamplesPerSecond * waveformat.BlockAlignment;

			m_BytesPerSample = waveformat.BitsPerSample / 8 * waveformat.Channels;
			m_Channels = waveformat.Channels;

			m_NumChunks = 2;
			m_ChunkSize = 2048;
			m_ChunkSizeBytes = m_ChunkSize * m_BytesPerSample;

			SoundBufferDescription bufferDesc = new SoundBufferDescription();
			bufferDesc.Format = waveformat;
			bufferDesc.Flags = BufferFlags.GetCurrentPosition2 | BufferFlags.GlobalFocus;
			bufferDesc.SizeInBytes = m_NumChunks * m_ChunkSizeBytes;

			m_Buffer = new SecondarySoundBuffer(m_SoundDevice, bufferDesc);

			m_Buffer.Play(0, PlayFlags.Looping);

			m_PlayingThread = new Thread(new ThreadStart(AudioThread));
			m_PlayingThread.IsBackground = true;
			m_PlayingThread.Priority = ThreadPriority.AboveNormal;
			m_PlayingThread.Start();

			m_Started = true;
		}

		private void AudioThread()
		{
			int lastChunk = -1;
			int currentChunk = -1;

			Sample[] buffer = new Sample[m_ChunkSize];
			FillBufferEventArgs fillBufferArgs = new FillBufferEventArgs(buffer);

			while (!m_Abort)
			{
				int cursor = m_Buffer.CurrentPlayPosition;

				int chunk = cursor / m_ChunkSizeBytes;

				if (chunk == lastChunk)
				{
					Thread.Sleep(1000 * ((chunk + 1) * m_ChunkSizeBytes - cursor) / m_BytesPerSample / m_SampleRate);
				}
				else
				{
					// Fill next buffer:
					currentChunk = (chunk + 1) % m_NumChunks;

					for (int i = 0; i < buffer.Length; i++) buffer[i] = Sample.Zero;

					FillBuffer(fillBufferArgs);

					m_Buffer.Write(buffer, currentChunk * m_ChunkSizeBytes, LockFlags.None);
				}
				lastChunk = chunk;
			}

			m_Buffer.Dispose();
		}

		protected override void Dispose(bool disposing)
		{
			m_Abort = true;

			base.Dispose(disposing);
		}

	}
}
