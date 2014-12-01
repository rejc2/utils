using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Diagnostics;
using System.Threading;

namespace Rejc2.Utils.Audio
{
	public class WavefileWavLoader : WavefileLoader
	{
		class FourByteId
		{
			readonly byte[] m_Data = new byte[4];

			private string m_TypeId;
			public string TypeId
			{
				get
				{
					if (m_TypeId == null)
					{
						m_TypeId = Encoding.ASCII.GetString(m_Data, 0, 4);
					}
					return m_TypeId;
				}
			}

			public bool Equals(FourByteId other)
			{
				return m_Data[0] == other.m_Data[0] && m_Data[1] == other.m_Data[1] &&
					m_Data[2] == other.m_Data[2] && m_Data[3] == other.m_Data[3];
			}

			public override bool Equals(object obj)
			{
				return Equals((FourByteId)obj);
			}

			public override int GetHashCode()
			{
				return TypeId.GetHashCode();
			}

			public FourByteId(SafeStream stream)
			{
				stream.Read(m_Data, 0, 4);
			}

			public FourByteId(string id)
			{
				m_TypeId = id;
				Encoding.ASCII.GetBytes(id).CopyTo(m_Data, 0);
			}

			public override string ToString()
			{
				return TypeId;
			}
		}

		class ChunkHeader
		{
			private FourByteId m_TypeId;
			public FourByteId TypeId
			{
				get { return m_TypeId; }
			}

			private int m_Length;
			public int Length
			{
				get { return m_Length; }
			}

			SafeStream m_Stream;


			public ChunkHeader(SafeStream stream)
			{
				m_Stream = stream;
				m_TypeId = new FourByteId(stream);
				m_Length = ReadI4(m_Stream);
			}

			public bool IsType(FourByteId type)
			{
				//return memcmp(id, type, 4)==0 ;
				return type.Equals(m_TypeId);
			}

			public SafeStream GetSubStream()
			{
				FixedLengthSubStream subStream = new FixedLengthSubStream(m_Stream, Length);
				SafeStream safeStream = new SafeStream(subStream);
				return safeStream;
			}

		}

		int m_Length;

		bool m_DoneFormat;
		int fmt_Channels;
		int fmt_SampleRate;
		int fmt_BitsPerSample;

		//bool m_Done;

		static readonly FourByteId RiffId = new FourByteId("RIFF");
		static readonly FourByteId WaveId = new FourByteId("WAVE");
		static readonly FourByteId FormatId = new FourByteId("fmt ");
		static readonly FourByteId DataId = new FourByteId("data");


		public override Wavefile Load(Stream fileStream)
		{
			SafeStream safeStream = new SafeStream(fileStream);
			try
			{
				ChunkHeader riffHeader = new ChunkHeader(safeStream);
				if (!riffHeader.IsType(RiffId)) throw new InvalidFileFormatException("WAV-file does not begin with `RIFF'.");
				SafeStream stream = riffHeader.GetSubStream();
				try
				{
					FourByteId waveHeader = new FourByteId(stream);
					if (!waveHeader.Equals(WaveId)) throw new InvalidFileFormatException("Riff file is not a waveform.");

					while (true) //!m_Done)
					{
						ChunkHeader chunk = new ChunkHeader(stream);

						if (chunk.IsType(FormatId))
						{
							using (SafeStream formatStream = chunk.GetSubStream())
							{
								int formatTag = ReadI2(formatStream);
								if (formatTag != 1) throw new InvalidFileFormatException("Wavefile is not PCM.");

								fmt_Channels = ReadI2(formatStream);
								//if (fmt_channels != 1) throw "Wavefile must be mono." ;
								//^^^ Fixed 19.Mar.2001

								fmt_SampleRate = ReadI4(formatStream);

								ReadI4(formatStream);  // <-- average bytes per second (rubbish)
								ReadI2(formatStream);  // <-- block align (who knows)

								fmt_BitsPerSample = ReadI2(formatStream);

								m_DoneFormat = true;
							}
						}
						else if (chunk.IsType(DataId))
						{
							if (!m_DoneFormat) throw new InvalidFileFormatException("No format block before data!");

							Wavefile wave = new Wavefile();
							wave.m_SampleRate = (float)fmt_SampleRate;

							switch (fmt_BitsPerSample)
							{
								case 8:
									{
										if (fmt_Channels == 2)
										{
											//read_pcm_8bit_st (f, chunk.len/2, wave) ;
											//read_pcm = read_pcm_8bit_st ;
											m_Length = chunk.Length / 2;
											//start() ;
										}
										else
										{
											//read_pcm_8bit (f, chunk.len, wave) ;
											//read_pcm = read_pcm_8bit ;
											m_Length = chunk.Length;
											//start() ;
										}
									} break;

								case 16:
									{
										if (fmt_Channels == 2)
										{
											//read_pcm_16bit_i_st (f, chunk.len/2/2, wave) ;
											//read_pcm = read_pcm_16bit_i_st ;
											m_Length = chunk.Length / 2 / 2;
											//start() ;
										}
										else
										{
											//read_pcm_16bit_i (f, chunk.len/2, wave) ;
											//read_pcm = read_pcm_16bit_i ;
											m_Length = chunk.Length / 2;
											//start() ;
										}
									}
									break;

								default: throw new InvalidFileFormatException("Must be 8- or 16-bit.");
							}

							ThreadPool.QueueUserWorkItem(delegate
							{
								using (SafeStream dataStream = chunk.GetSubStream())
								{
									ReadPcm(wave, dataStream, m_Length, fmt_BitsPerSample, fmt_Channels, true);
								}

								//m_Done = true;
								wave.FinishedLoading = true;
							});
							safeStream = null;
							stream = null;
							return wave;
						}
						else
						{
							chunk.GetSubStream().Dispose();

							Trace.WriteLine(String.Format("Warning: Unknown chunk `{0}'", chunk.TypeId)); ;
						}
					}
				}
				finally
				{
					if (stream != null)
						stream.Dispose();
				}
			}
			finally
			{
				if (safeStream != null)
					safeStream.Dispose();
			}
		}


	}
}
