using System.Xml.Serialization;
using Common.Preferences;
using CSCore.CoreAudioAPI;
using CSCore.SoundOut;

namespace Common.AudioPlayer
{
	[Serializable, XmlType(TypeName = "Settings")]
	public class AudioPreferences:PreferenceBase<AudioPreferences>
	{
		public static readonly string SettingsPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Vixen", "AudioDevice.settings");

		#region Overrides of PreferenceBase

		/// <inheritdoc />
		public override void SetStandardValues()
		{
			SoundOutDeviceId = AudioOutputManager.DefaultDevicePlaceholder;
			AudioOutputMode = WasapiOut.IsSupportedOnCurrentPlatform ? AudioOutputMode.WASAPI : AudioOutputMode.DirectSound;
			AudioClientShareMode = AudioClientShareMode.Shared;
			Latency = 25;
		}

		#endregion

		public string SoundOutDeviceId { get; set; }

		public AudioOutputMode AudioOutputMode { get; set; }

		public AudioClientShareMode AudioClientShareMode { get; set; } = AudioClientShareMode.Shared;

		public int Latency { get; set; } = 25;
	}
}
