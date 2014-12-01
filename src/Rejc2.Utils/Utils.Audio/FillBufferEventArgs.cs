using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rejc2.Utils.Audio
{
	public class FillBufferEventArgs : EventArgs
	{
		public readonly Sample[] Buffer;

		public FillBufferEventArgs(Sample[] buffer)
		{
			Buffer = buffer;
		}
	}

}
