using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Rejc2.Utils.Audio
{
	public sealed class SafeStream : Stream
	{
		public class EndOfFileException : Exception
		{
		}

		private Stream m_Stream;
		public Stream Stream
		{
			get { return m_Stream; }
		}

		public SafeStream(Stream stream)
		{
			m_Stream = stream;
		}

		public override bool CanRead
		{
			get { return m_Stream.CanRead; }
		}

		public override bool CanSeek
		{
			get { return m_Stream.CanSeek; }
		}

		public override bool CanWrite
		{
			get { return m_Stream.CanWrite; }
		}

		public override void Flush()
		{
			m_Stream.Flush();
		}

		public override long Length
		{
			get { return m_Stream.Length; }
		}

		public override long Position
		{
			get
			{
				return m_Stream.Position;
			}
			set
			{
				m_Stream.Position = value;
			}
		}

		public override int Read(byte[] buffer, int offset, int count)
		{
			// Note: "An implementation is free to return fewer bytes than requested even if the end
			//       of the stream has not been reached."

			int bytesRead=0;
			while (count > 0)
			{
				int read = m_Stream.Read(buffer, offset, count);
				if (read == 0) throw new EndOfFileException();
				bytesRead += read;
				offset += read;
				count -= read;
			}
			return bytesRead;
		}

		public override int ReadByte()
		{
			int read = m_Stream.ReadByte();
			if (read < 0) throw new EndOfFileException();
			return read;
		}

		public override long Seek(long offset, SeekOrigin origin)
		{
			return m_Stream.Seek(offset, origin);
		}

		public override void SetLength(long value)
		{
			m_Stream.SetLength(value);
		}

		public override void Write(byte[] buffer, int offset, int count)
		{
			m_Stream.Write(buffer, offset, count);
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				m_Stream.Dispose();
			}

			base.Dispose(disposing);
		}
	}
}
