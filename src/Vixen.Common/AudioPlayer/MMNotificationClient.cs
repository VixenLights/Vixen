using NAudio.CoreAudioApi;
using NAudio.CoreAudioApi.Interfaces;

namespace Common.AudioPlayer
{
	/// <summary>
	/// The <see cref="MMNotificationClient"/> object provides notifications when an audio endpoint device is added or removed, when the state or properties of an endpoint device change, or when there is a change in the default role assigned to an endpoint device.
	/// </summary>
	internal class MMNotificationClient : IMMNotificationClient
	{
		/// <summary>
		/// Occurs when the state of an audio endpoint device has changed.
		/// </summary>
		public event EventHandler<DeviceStateChangedEventArgs> DeviceStateChanged;

		/// <summary>
		/// Occurs when a new audio endpoint device has been added.
		/// </summary>
		public event EventHandler<DeviceNotificationEventArgs> DeviceAdded;

		/// <summary>
		/// Occurs when an audio endpoint device has been removed.
		/// </summary>
		public event EventHandler<DeviceNotificationEventArgs> DeviceRemoved;

		/// <summary>
		/// Occurs when the default audio endpoint device for a particular device role has changed.
		/// </summary>
		public event EventHandler<DefaultDeviceChangedEventArgs> DefaultDeviceChanged;

		/// <summary>
		/// Occurs when the value of a property belonging to an audio endpoint device has changed.
		/// </summary>
		public event EventHandler<DevicePropertyChangedEventArgs> DevicePropertyChanged;

		/// <summary>
		/// Initializes a new instance of the <see cref="MMNotificationClient"/> class.
		/// </summary>
		public MMNotificationClient()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="MMNotificationClient"/> class based on an existing <see cref="MMDeviceEnumerator"/>.
		/// </summary>
		/// <param name="enumerator"></param>
		public MMNotificationClient(MMDeviceEnumerator enumerator)
		{
			if (enumerator == null)
				throw new ArgumentNullException("enumerator");

			Initialize(enumerator);
		}

		private void Initialize(MMDeviceEnumerator enumerator)
		{
			int result = enumerator.RegisterEndpointNotificationCallback(this);
			CheckError(result, "IMMDeviceEnumerator", "RegisterEndpointNotificationCallback");
		}

		public static void CheckError(int result, string interfaceName, string member)
		{
			// < 0 means error, >= 0 means success
			if (result < 0)
				throw new Exception($"Result:{result} Interface:{interfaceName} Member:{member}");
		}

		/// <summary>
		/// The OnDeviceStateChanged method indicates that the state of an audio endpoint device has
		/// changed.
		/// </summary>
		/// <param name="deviceId">The device id that identifies the audio endpoint device.</param>
		/// <param name="deviceState">Specifies the new state of the endpoint device.</param>
		public void OnDefaultDeviceChanged(DataFlow flow, Role role, string defaultDeviceId)
		{
			DefaultDeviceChanged?.Invoke(this, new DefaultDeviceChangedEventArgs(defaultDeviceId, flow, role));
		}

		public void OnDeviceAdded(string pwstrDeviceId)
		{
			DeviceAdded?.Invoke(this, new DeviceNotificationEventArgs(pwstrDeviceId));
		}

		public void OnDeviceRemoved(string deviceId)
		{
			DeviceRemoved?.Invoke(this, new DeviceNotificationEventArgs(deviceId));
		}

		public void OnDeviceStateChanged(string deviceId, DeviceState newState)
		{
			DeviceStateChanged?.Invoke(this, new DeviceStateChangedEventArgs(deviceId, newState));
		}

		public void OnPropertyValueChanged(string pwstrDeviceId, PropertyKey key)
		{
			DevicePropertyChanged?.Invoke(this, new DevicePropertyChangedEventArgs(pwstrDeviceId, key));
		}
	}
}
