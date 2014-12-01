using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Utils.WindowsForms
{
	[Flags]
	public enum FileFolderSelectionTypes
	{
		Folder = 1 << 0,
		Save = 1 << 1
	}
}
