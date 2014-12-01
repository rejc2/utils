using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using Rejc2.Utils;
using Rejc2.Utils.Audio;
using System.Threading;

namespace Rejc2.Utils.Audio
{
	public class Wavefile
	{
		//internal List<Sample> m_Samples;
		//internal Sample[] m_SampleArray;
		internal BlockArray<Sample> m_Samples;
		internal float m_SampleRate;
		public float SampleRate
		{
			get { return m_SampleRate; }
		}

		internal Wavefile()
		{
		}

		public static readonly Wavefile Blank = new Wavefile() { m_FinishedLoading = true, m_SampleRate = 44100, m_Samples = new BlockArray<Sample>(1, 0) };

		public int NumberOfSamples
		{
			get { return m_Samples.Count; }
		}

		public double Duration
		{
			get
			{
				return (double)NumberOfSamples / SampleRate;
			}
		}

		private readonly object m_Lock = new object();
		private volatile bool m_FinishedLoading;

		public bool FinishedLoading
		{
			[System.Diagnostics.DebuggerStepThrough]
			get { return m_FinishedLoading; }
			internal set
			{
				if (value)
				{
					lock (m_Lock)
					{
						m_FinishedLoading = true;
						Monitor.PulseAll(m_Lock);
					}
				}
			}
		}

		public void WaitUntilLoaded()
		{
			if (m_FinishedLoading)
				return;

			lock (m_Lock)
			{
				while (!m_FinishedLoading)
				{
					Monitor.Wait(m_Lock);
				}
			}
		}

		public void GetFloatData(int start, float[] buffer, int offset, int count)
		{
			int i = 0;
			for (; i<count && i + start < 0; i++)
			{
				buffer[offset + i] = 0.0F;
			}
			if (m_Samples.Count - start - i > 0)
			{
				Sample[] samples = new Sample[Math.Min(count - i, m_Samples.Count - start - i)];
				m_Samples.Read(start + i, samples);
				for (int j = 0; j < samples.Length; i++, j++)
				{
					buffer[offset + i] = (float)(samples[j].Left + samples[j].Right);
				}
			}
			//for (; i < count && i+start<m_Samples.Count; i++)
			//{
			//    buffer[offset + i] = (float)(m_Samples[start + i].Left + m_Samples[start+i].Right);
			//}
			for (; i < count; i++)
			{
				buffer[offset + i] = 0.0F;
			}

		}

		public void GetSampleData(int start, Sample[] buffer, int offset, int count)
		{
			int i = 0;
			for (; i < count && i + start < 0; i++)
			{
				buffer[offset + i] = Sample.Zero;
			}
			//for (; i < count && i + start < m_Samples.Count; i++)
			//{
			//    buffer[offset + i] = m_Samples[start + i];
			//}
			int readIn = Math.Min(count-i, m_Samples.Count - start - i);
			if (readIn > 0)
			{
				m_Samples.Read(start + i, buffer, offset + i, readIn);
				i += readIn;
			}
			for (; i < count; i++)
			{
				buffer[offset + i] = Sample.Zero;
			}
		}

		//public void GetSampleData(int start, short[] buffer, int offset, int count)
		//{
		//    if (m_SampleArray == null)
		//        m_SampleArray = m_Samples.ToArray();

		//    count -= 1;
		//    int i = 0;
		//    for (; i < count && i / 2 + start < 0; i += 2)
		//    {
		//        buffer[offset + i] = 0;
		//        buffer[offset + i + 1] = 0;
		//    }
		//    for (; i < count && i / 2 + start < m_SampleArray.Length; i += 2)
		//    {
		//        buffer[offset + i] = m_SampleArray[start + i / 2].Left;
		//        buffer[offset + i + 1] = m_SampleArray[start + i / 2].Right;
		//    }
		//    for (; i < count; i += 2)
		//    {
		//        buffer[offset + i] = 0;
		//        buffer[offset + i + 1] = 0;
		//    }
		//}

		//public unsafe void GetSampleData(int start, uint[] buffer, int offset, int count)
		//{
		//    if (m_SampleArray == null)
		//        m_SampleArray = m_Samples.ToArray();

		//    int i = 0;
		//    for (; i < count && i + start < 0; i ++)
		//    {
		//        buffer[offset + i] = 0;
		//    }
		//    fixed (Sample* samples = m_SampleArray)
		//    {
		//        uint* intSamples = (uint*)samples;
		//        intSamples += start;
		//        for (; i < count && i + start < m_SampleArray.Length; i ++, intSamples++)
		//        {
		//            buffer[offset + i] = *intSamples;
		//        }
		//    }
		//    for (; i < count; i ++)
		//    {
		//        buffer[offset + i] = 0;
		//    }
		//}
	}
}
