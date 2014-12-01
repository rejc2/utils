using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Rejc2.Utils
{
	public static class StreamUtils
	{
		public static MemoryStream CopyToMemoryStream(this Stream stream)
		{
			if (stream == null) throw new ArgumentNullException("stream");
			if (!stream.CanRead)
				throw new ArgumentOutOfRangeException("stream.CanRead", stream.CanRead, " ");

			var memStream = new MemoryStream();
			byte[] buffer = new byte[4096];

			while (true)
			{
				int count = stream.Read(buffer, 0, buffer.Length);
				if (count == 0)
					break;
				memStream.Write(buffer, 0, count);
			}

			memStream.Position = 0;
			return memStream;
		}
	}
}
