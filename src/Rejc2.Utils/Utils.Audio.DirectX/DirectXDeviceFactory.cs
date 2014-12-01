using System;
using System.Collections.Generic;
using System.Text;
using Rejc2.Utils.Audio;

namespace Rejc2.Utils.Audio.DirectX
{
	public class DirectXDeviceFactory : IAudioDeviceProvider
	{
		IntPtr m_Owner;

		public DirectXDeviceFactory(IntPtr owner)
		{
			m_Owner = owner;
		}

		#region IAudioDeviceProvider Members

		public AudioDevice CreateAudioDevice(AudioSource source)
		{
			return new DirectXDevice(source, m_Owner);
		}

		#endregion
	}
}
