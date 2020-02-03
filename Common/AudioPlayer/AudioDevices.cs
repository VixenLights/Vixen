using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using CSCore.CoreAudioAPI;
using NLog;

namespace Common.AudioPlayer
{
	public class AudioDevices
	{
		private static Logger Logging = LogManager.GetCurrentClassLogger();
		private static readonly MMDeviceEnumerator DeviceEnumerator;
		private static readonly string _devicePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Vixen", "AudioDevice.dat");
		
		static AudioDevices()
		{
			DeviceEnumerator = new MMDeviceEnumerator();
			NotificationClient = new MMNotificationClient(DeviceEnumerator);
		}

		/// <summary>
		/// Find the device by id or the default device if the id does not exist.
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		public static MMDevice GetDevice(string id)
		{
			try
			{
				return DeviceEnumerator.GetDevice(id);
			}
			catch (Exception e)
			{
				Logging.Error(e, $"Unable to find device id:{id}");
			}

			return GetDefaultMMDevice();
		}

		public static MMDevice GetDefaultAudioEndpoint()
		{
			return DeviceEnumerator.GetDefaultAudioEndpoint(DataFlow.Render, Role.Multimedia);
		}

		public static Device GetDeviceOrDefault(string id)
		{
			try
			{
				var mmDevice = string.IsNullOrEmpty(id) ? GetDefaultMMDevice() : DeviceEnumerator.GetDevice(id);
				var dd = GetDefaultDevice();
				if (mmDevice != null)
				{
					return new Device(mmDevice.FriendlyName, mmDevice.DeviceID, mmDevice.DeviceID == dd.Id);
				}

				return dd;
			}
			catch (Exception ex)
			{
				Logging.Error(ex, $"Unable to get device by id:{id}. Using the default.");
			}

			return GetDefaultDevice();
		}

		public static Device GetDefaultDevice()
		{
			var mmDevice = GetDefaultMMDevice();
			return new Device(mmDevice.FriendlyName, mmDevice.DeviceID, true);
		}
		
		public static List<Device> GetActiveOutputDevices()
		{
			var mmDevices = DeviceEnumerator.EnumAudioEndpoints(DataFlow.Render, DeviceState.Active);
			var defaultDevice = GetDefaultAudioEndpoint();
			var devices = new List<Device>();
			foreach (var device in mmDevices)
			{
				devices.Add(new Device(device.FriendlyName, device.DeviceID ,device.DeviceID == defaultDevice.DeviceID));
			}

			return devices;
		}

		private static MMDevice GetDefaultMMDevice()
		{
			return DeviceEnumerator.GetDefaultAudioEndpoint(DataFlow.Render, Role.Multimedia);
		}

		public static MMNotificationClient NotificationClient { get; set; }

		public static string PreferredAudioDeviceId
		{
			get
			{
				string id = String.Empty;

				try
				{
					id = File.ReadAllText(_devicePath);
				}
				catch (Exception e)
				{
					Logging.Warn(e, "Unable to read audio device id. Useing the default device.");
				}

				var d = GetDeviceOrDefault(id);
				return d.Id;
			}

			set
			{
				try
				{
					File.WriteAllText(_devicePath, value);
				}
				catch (Exception e)
				{
					Logging.Error(e, "Unable to write audio device id.");
				}
			}
		}
	}
}
