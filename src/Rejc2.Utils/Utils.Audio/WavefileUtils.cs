using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.IO;

namespace Rejc2.Utils.Audio
{
	public static class WavefileUtils
	{
		public static Wavefile Normalise(this Wavefile wave)
		{
			if (wave == null) throw new ArgumentNullException("wave");

			Wavefile newWave = new Wavefile();
			newWave.m_SampleRate = wave.SampleRate;
			newWave.m_Samples = new BlockArray<Sample>(12);

			ThreadPool.QueueUserWorkItem(new WaitCallback(delegate
			{
				wave.WaitUntilLoaded();

				int peak = 0;
				foreach (var sample in wave.m_Samples)
				{
					peak = Math.Max(peak, Math.Max(Math.Abs(sample.Left), Math.Abs(sample.Right)));
				}

				int factor = 256 * 32767 / peak;

				Sample[] newSample = new Sample[1];
				foreach (var sample in wave.m_Samples)
				{
					newSample[0] = new Sample((short)((sample.Left * factor) >> 8),
						(short)((sample.Right * factor) >> 8));
					newWave.m_Samples.Write(newSample);
				}

				newWave.FinishedLoading = true;
			}));
			//thread.Start();

			return newWave;
		}

		public static TimeSpan AudioLength(this Wavefile wave, float level)
		{
			if (wave == null) throw new ArgumentNullException("wave");
			if (level < 0 || level > 1)
				throw new ArgumentOutOfRangeException("level", level, " ");

			wave.WaitUntilLoaded();

			short shLevel = (short)(32767 * level);
			int lastPoint = 0;
			int i = 0;
			foreach (var sample in wave.m_Samples)
			{
				if (sample.Left <= -shLevel || sample.Left >= shLevel ||
						sample.Right <= -shLevel || sample.Right >= shLevel)
				{
					lastPoint = i;
				}

				i++;
			}

			return TimeSpan.FromSeconds(lastPoint / (double)wave.SampleRate);
		}

		//public static void Save(string fileName)
		//{
		//  if (fileName == null) throw new ArgumentNullException("fileName");

		//  using (var stream = new FileStream(fileName, FileMode.Create, FileAccess.Write, FileShare.None))
		//  using (var writer = new MyBinaryWriter(stream))
		//  {
		//    Save(writer);
		//  }
		//}

		//private static void WriteSimpleString(MyBinaryWriter writer, string s)
		//{
		//  if (writer == null) throw new ArgumentNullException("writer");
		//  if (s == null) throw new ArgumentNullException("s");

		//  foreach (var c in s)
		//  {
		//    writer.Write((byte)c);
		//  }
		//}

		//public static void Save(MyBinaryWriter writer)
		//{
		//  if (writer == null) throw new ArgumentNullException("writer");

		//  WriteSimpleString(writer, "RIFF");
		//}
	}
}
