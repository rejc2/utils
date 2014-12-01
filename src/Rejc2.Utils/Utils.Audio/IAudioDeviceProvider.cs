using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rejc2.Utils.Audio
{
	public interface IAudioDeviceProvider
	{
		AudioDevice CreateAudioDevice(AudioSource source);
	}
}
