using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Rejc2.Utils.Audio
{
	class FixedLengthSubStream : Stream
	{
		private Stream m_Stream;
		public Stream Stream
		{
			get { return m_Stream; }
		}

		private long m_Length;
		public override long Length
		{
			get { return m_Length; }
		}

		private long m_Position;


		public FixedLengthSubStream(Stream stream, long length)
		{
			m_Stream = stream;
			m_Length = length;
			m_Position = 0;
		}

		public override bool CanRead
		{
			get { return m_Stream.CanRead; }
		}

		public override bool CanSeek
		{
			get { return false; }
		}

		public override bool CanWrite
		{
			get { return false; }
		}

		public override void Flush()
		{
			m_Stream.Flush();
		}

		public override long Position
		{
			get
			{
				return m_Position;
			}
			set
			{
				throw new NotSupportedException();
			}
		}

		public override int Read(byte[] buffer, int offset, int count)
		{
			if (count > m_Length - m_Position) count = (int)(m_Length - m_Position);

			int bytesRead = m_Stream.Read(buffer, offset, count);
			m_Position += bytesRead;
			return bytesRead;
		}

		public override int ReadByte()
		{
			if (m_Position >= m_Length) return -1;
			int byteRead = m_Stream.ReadByte();
			if (byteRead >= 0) m_Position++;
			return byteRead;
		}

		public override long Seek(long offset, SeekOrigin origin)
		{
			throw new NotSupportedException();
		}

		public override void SetLength(long value)
		{
			throw new NotSupportedException();
		}

		public override void Write(byte[] buffer, int offset, int count)
		{
			throw new NotSupportedException();
		}

		const int scratchBufferSize = 4096;
		static readonly byte[] scratchBuffer = new byte[scratchBufferSize];

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				// If we have not exhausted the stream, we should:
				while (m_Position < m_Length)
				{
					int read = Read(scratchBuffer, 0, scratchBufferSize);
					if (read == 0) break;
				}
			}

			base.Dispose(disposing);
		}
	}
}
