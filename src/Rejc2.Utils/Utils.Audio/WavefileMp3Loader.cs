using System;
using System.Collections.Generic;
using System.Text;
using Mp3Sharp;
using System.IO;
using System.Threading;
using Rejc2.Utils;
using Rejc2.Utils.Audio;

namespace Rejc2.Utils.Audio
{
	public class WavefileMp3Loader : WavefileLoader
	{
		private delegate void DoLoadingDelegate(Wavefile wave, Mp3Stream stream);

		Wavefile m_Wavefile;

		void DoLoading(object obj)
		{
			Wavefile wave = m_Wavefile;
			Mp3Stream stream = (Mp3Stream)obj;

			BlockArray<Sample> array = wave.m_Samples;

			//SafeStream safeStream = new SafeStream(stream);

			Sample[] sample = new Sample[10000];
			byte[] buffer = new byte[sample.Length * 4];

			Thread.Sleep(1000);

			while (true) //safeStream.Position < safeStream.Length)
			{
				//int count;
				//int countMax = (int)Math.Min(1024, (safeStream.Length - safeStream.Position)/4);
				//for (count=0; count <countMax; count ++)
				//{
				//    //sample[count] = new Sample(ReadI2(safeStream), ReadI2(safeStream));
				//    sample[count] = ReadSI2(safeStream);
				//}

				int byteCount = stream.Read(buffer, 0, buffer.Length);
				if (byteCount == 0)
					break;

				int sampleCount = byteCount / 4;

				for (int i = 0, j=0; i < sampleCount; i ++, j+=4)
				{
					sample[i] = new Sample((short)(buffer[j] | buffer[j + 1] << 8),
												(short)(buffer[j + 2] | buffer[j + 3] << 8));
				}

				array.Write(sample, 0, sampleCount);
				Thread.Sleep(0);
			}

			stream.Dispose();
			wave.FinishedLoading = true;
		}

		public override Wavefile Load(Stream fileStream)
		{
			Mp3Stream mp3Stream = new Mp3Stream(fileStream);

			if (mp3Stream.Frequency < 0) mp3Stream.DecodeFrames(1);
			if (!(mp3Stream.Frequency > 0 && mp3Stream.ChannelCount > 0))
				throw new InvalidFileFormatException("No frequency/channel information");

			if (mp3Stream.Format != SoundFormat.Pcm16BitStereo)
				throw new NotImplementedException("Only stereo MP3 supported");

			Wavefile wave = new Wavefile();
			wave.m_SampleRate = mp3Stream.Frequency;
			wave.m_Samples = new BlockArray<Sample>(12);

			m_Wavefile = wave;

			//new DoLoadingDelegate(DoLoading).BeginInvoke(wave, mp3Stream, new AsyncCallback(FinishedLoadingCallback), wave);

			Thread loadThread = new Thread(new ParameterizedThreadStart(DoLoading));
			loadThread.IsBackground = true;
			loadThread.Start(mp3Stream);

			//DoLoading(wave, mp3Stream);

			return wave;
		}

		//void FinishedLoadingCallback(IAsyncResult result)
		//{
		//    Wavefile wave = (Wavefile)result.AsyncState;

		//}
	}
}
