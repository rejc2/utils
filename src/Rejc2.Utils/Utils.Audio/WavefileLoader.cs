using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Rejc2.Utils;
using Rejc2.Utils.Audio;

namespace Rejc2.Utils.Audio
{
	public abstract class WavefileLoader
	{
		readonly byte[] m_ByteBuffer = new byte[16];

		static protected int ReadI4(SafeStream stream)
		{
			return stream.ReadByte() | (stream.ReadByte() << 8) | (stream.ReadByte() << 16) | (stream.ReadByte() << 24);
		}

		protected short ReadI2(SafeStream stream)
		{
			//return (short)(stream.ReadByte() | (stream.ReadByte() << 8));
			stream.Read(m_ByteBuffer, 0, 2);
			return (short)(m_ByteBuffer[0] | (m_ByteBuffer[1] << 8));
		}

		protected Sample ReadSI2(SafeStream stream)
		{
			stream.Read(m_ByteBuffer, 0, 4);
			return new Sample((short)(m_ByteBuffer[0] | (m_ByteBuffer[1] << 8)),
				(short)(m_ByteBuffer[2] | (m_ByteBuffer[3] << 8)));
		}

		//static const int BufSize = 4096;

		//static int f_read_pcm_8bit(Wavefile::Sample* idata, Stream* file, int len)
		//{
		//    const int BPS = 1;
		//    byte[] buf = new byte[BufSize * BPS];
		//    int numread = file->read(buf, len * BPS) / BPS;

		//    for (int i = 0; i < numread; i++)
		//    {
		//        idata[i].ch[0] = idata[i].ch[1] = (Int16)((read_i2(buf + i * BPS) - 128) * 256);
		//    }
		//    return numread;
		//}
		//static int f_read_pcm_8bit_st(Wavefile::Sample* idata, Stream* file, int len)
		//{
		//    const int BPS = 2;
		//    byte[] buf = new byte[BufSize * BPS];
		//    int numread = file->read(buf, len * BPS) / BPS;

		//    for (int i = 0; i < numread; i++)
		//    {
		//        idata[i].ch[0] = (Int16)((read_i2(buf + i * BPS) - 128) * 256);
		//        idata[i].ch[1] = (Int16)((read_i2(buf + i * BPS) - 128) * 256);
		//    }
		//    return numread;
		//}
		////static int f_read_pcm_16bit_m(Wavefile::Sample* idata, Stream* file, int len)
		////{
		////    const int BPS = 2;
		////    byte[] buf = new byte[BufSize * BPS];
		////    int numread = file->read(buf, len * BPS) / BPS;

		////    for (int i = 0; i < numread; i++)
		////    {
		////        idata[i].ch[0] = idata[i].ch[1] = (Int16)read_m2(buf + i * BPS);
		////    }
		////    return numread;
		////}
		////static int f_read_pcm_16bit_m_st(Wavefile::Sample* idata, Stream* file, int len)
		////{
		////    const int BPS = 4;
		////    byte[] buf = new byte[BufSize * BPS];
		////    int numread = file->read(buf, len * BPS) / BPS;

		////    for (int i = 0; i < numread; i++)
		////    {
		////        idata[i].ch[0] = (Int16)read_m2(buf + i * BPS);
		////        idata[i].ch[1] = (Int16)read_m2(buf + i * BPS + 2);
		////    }
		////    return numread;
		////}
		//static int f_read_pcm_16bit_i(Wavefile::Sample* idata, Stream* file, int len)
		//{
		//    const int BPS = 2;
		//    byte[] buf = new byte[BufSize * BPS];
		//    int numread = file->read(buf, len * BPS) / BPS;

		//    for (int i = 0; i < numread; i++)
		//    {
		//        idata[i].ch[0] = idata[i].ch[1] = (Int16)ReadI2(buf + i * BPS);
		//    }
		//    return numread;
		//}
		//static int f_read_pcm_16bit_i_st(Wavefile::Sample* idata, Stream* file, int len)
		//{
		//    const int BPS = 4;
		//    byte[] buf = new byte[BufSize * BPS];
		//    int numread = file->read(buf, len * BPS) / BPS;

		//    for (int i = 0; i < numread; i++)
		//    {
		//        idata[i].ch[0] = (Int16)ReadI2(buf + i * BPS);
		//        idata[i].ch[1] = (Int16)ReadI2(buf + i * BPS + 2);
		//    }
		//    return numread;
		//}

		private delegate int ReadFunction(List<Sample> data, SafeStream stream, int length);

		Sample ReadPcmI16Stereo(SafeStream stream)
		{
			return new Sample(ReadI2(stream), ReadI2(stream));
		}

		Sample ReadPcmI16Mono(SafeStream stream)
		{
			short value = ReadI2(stream);
			return new Sample(value, value);
		}

		private delegate Sample ReadSample(SafeStream stream);

		protected void ReadPcm(Wavefile wave, SafeStream stream, int len,
								int bits, int channels, bool intel)
		{
			//if (!wave->dontdelete_) sample_free(wave->idata_) ;
			//wave.m_Samples = new List<Sample>(len);
			wave.m_Samples = new BlockArray<Sample>(12);

			//int (*readfun)(Wavefile::Sample *, Stream *, int) ;

			ReadSample readFunction;

			if (bits==16) {
				if (intel) // Intel byte order
				{
					if (channels==1) {
						//readfun = f_read_pcm_16bit_i ;
						readFunction = new ReadSample(ReadPcmI16Mono);
					} 
					else if (channels==2)
					{
						//readfun = f_read_pcm_16bit_i_st ;
						readFunction = new ReadSample(ReadPcmI16Stereo);
					}
					else throw new InvalidOperationException();
				}
				else // Motorola byte order
				{
					if (channels==1) {
						//readfun = f_read_pcm_16bit_m ;
						throw new NotImplementedException();
					}
					else if (channels==2)
					{
						//readfun = f_read_pcm_16bit_m_st ;
						throw new NotImplementedException();
					}
					else throw new InvalidOperationException();
				}
			}
			else if (bits==8)
			{
				if (channels==1) {
					//readfun = f_read_pcm_8bit ;
					throw new NotImplementedException();
				}
				else if (channels==2)
				{
					//readfun = f_read_pcm_8bit_st ;
					throw new NotImplementedException();
				}
				else throw new InvalidOperationException();
			}
			else throw new InvalidOperationException();

			//int a = 0 ;
			//while (a<len) {
			//    //int numread = readfun(wave->idata_+a, file, (len-a) <? BufSize) ;
			//    //if (numread==0) break ;
			//    a += numread ;
			//}

			Sample[] sample = new Sample[1];
			for (int i = 0; i < len; i++)
			{
				sample[0] = readFunction(stream);
				wave.m_Samples.Write(sample);
			}
		}



		public class InvalidFileFormatException : Exception
		{
			public InvalidFileFormatException()
			{
			}

			public InvalidFileFormatException(string message)
				: base(message)
			{
			}
		}

		public virtual Wavefile Load(FileInfo file)
		{
			Stream stream = file.Open(FileMode.Open, FileAccess.Read, FileShare.Read);
			{
				return Load(stream);
			}
		}

		public abstract Wavefile Load(Stream fileStream);
	}
}
