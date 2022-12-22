﻿using System.Collections.ObjectModel;

namespace Common.AudioPlayer
{
	public class AudioOutputProxy
	{
		public delegate AudioDevice GetAudioDevice(string deviceId);

		public string Name { get; set; }
		public ObservableCollection<AudioDevice> AudioDevices { get; set; }
		public AudioOutputMode AudioOutputMode { get; set; }

		private readonly GetAudioDevice _getAudioDeviceAction;
		public AudioOutputProxy(GetAudioDevice audioDeviceAction)
		{
			AudioDevices = new ObservableCollection<AudioDevice>();
			_getAudioDeviceAction = audioDeviceAction;
		}

		public void AddDevice(string deviceID)
		{
			var newItem = _getAudioDeviceAction.Invoke(deviceID);
			if (newItem != null) AudioDevices.Add(newItem);
		}
	}
}
