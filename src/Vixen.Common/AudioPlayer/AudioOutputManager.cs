using System.Collections.ObjectModel;
using System.Text;
using Microsoft.Win32;
using NAudio.CoreAudioApi;
using NAudio.Wave;
using NLog;

namespace Common.AudioPlayer
{
	public class AudioOutputManager : IDisposable
    {
	    private static readonly Logger Logging = LogManager.GetCurrentClassLogger();
	    private static AudioOutputManager _instance;
        public const string DefaultDevicePlaceholder = "-0";
        
        private string _currentDeviceId;

        private bool? _enabled;

        private string _lastDefaultDeviceChanged;
        private AudioPreferences _preferences;
        private readonly MMDeviceEnumerator _mmDeviceEnumerator;
        private readonly MMNotificationClient _mmNotificationClient;

        static AudioOutputManager()
        {
	        //CodecFactory.Instance.Register("ogg-vorbis", new CodecFactoryEntry(s => new NVorbisSource(s).ToWaveSource(), ".ogg"));
        }

        protected AudioOutputManager()
        {
            _mmDeviceEnumerator = new MMDeviceEnumerator();
            _mmNotificationClient = new MMNotificationClient(_mmDeviceEnumerator);
            Activate();
        }

        public static AudioOutputManager Instance()
        {
	        if (_instance == null)
	        {
                _instance = new AudioOutputManager();
	        }

	        return _instance;
        }

        public void Dispose()
        {
	        _mmNotificationClient.DefaultDeviceChanged -= _mmNotificationClient_DefaultDeviceChanged;
	        _mmNotificationClient.DeviceRemoved -= _mmNotificationClient_DeviceRemoved;
	        _mmNotificationClient.DeviceStateChanged -= _mmNotificationClient_DeviceStateChanged;
	        _mmNotificationClient.DeviceAdded -= _mmNotificationClient_DeviceAdded;
            SystemEvents.PowerModeChanged -= SystemEvents_PowerModeChanged;
        }

       public ObservableCollection<AudioDevice> AudioOutputList { get; set; }

        public event EventHandler RefreshAudioOutput;
        public event EventHandler Disable;
        public event EventHandler Enable;

        internal void Activate()
        {
	        _preferences = AudioPreferences.Load(AudioPreferences.SettingsPath);
	        _mmNotificationClient.DefaultDeviceChanged += _mmNotificationClient_DefaultDeviceChanged;
	        _mmNotificationClient.DeviceRemoved += _mmNotificationClient_DeviceRemoved;
	        _mmNotificationClient.DeviceStateChanged += _mmNotificationClient_DeviceStateChanged;
	        _mmNotificationClient.DeviceAdded += _mmNotificationClient_DeviceAdded;

            SystemEvents.PowerModeChanged += SystemEvents_PowerModeChanged;
            RefreshAudioOutputProxy();
            CheckCurrentState();
        }

        protected void OnRefreshSoundOut()
        {
	        RefreshAudioOutput?.Invoke(this, EventArgs.Empty);
        }

        public string AudioOutputDeviceId
        {
	        get => _preferences.SoundOutDeviceId;
	        set
	        {
		        _preferences.SoundOutDeviceId = value;
                _preferences.Save(AudioPreferences.SettingsPath);
                OnRefreshSoundOut();
	        }
        }

        public AudioOutputMode AudioOutputMode
        {
	        get => _preferences.AudioOutputMode;
	        set
	        {
		        _preferences.AudioOutputMode = value;
                _preferences.Save(AudioPreferences.SettingsPath);
                OnRefreshSoundOut();
	        }
        }

        public int Latency
        {
	        get => _preferences.Latency;
	        set
	        {
		        _preferences.Latency = value;
                _preferences.Save(AudioPreferences.SettingsPath);
	        }
        }

        public bool AudioClientShared
        {
	        get => _preferences.AudioClientShareMode == AudioClientShareMode.Shared;
	        set
	        {
		        _preferences.AudioClientShareMode = value?AudioClientShareMode.Shared:AudioClientShareMode.Exclusive;
		        _preferences.Save(AudioPreferences.SettingsPath);
	        }
        }

        public static string GetSupportedFilesFilter()
        {
			var stringBuilder = new StringBuilder();
			stringBuilder.Append("Supported Files|");
			stringBuilder.Append(String.Concat(GetSupportedFileExtensions().Select(x => "*" + x + ";").ToArray()));
			stringBuilder.Remove(stringBuilder.Length - 1, 1);
			return stringBuilder.ToString();
        }

        public static string[] GetSupportedFileExtensions()
        {
            List<string> ext = new List<string>() { "mp3", "mpeg3", "wav", "wave", "flac", "fla", "aiff", "aif", "aifc", "aac", "adt",
                "adts", "m2ts", "mp2", "3g2", "3gp2", "3gp", "3gpp", "m4a", "m4v", "mp4v", "mp4", "mov",  "asf", "wm", "wmv", "wma", "mp1", "avi", "ac3", "ec3", "ogg"};
	       
	        return ext.Select(x => "." + x).ToArray();
        }

        public IWavePlayer GetAudioOutput()
        {
            MMDevice defaultDevice;
            
            try
            {
                defaultDevice = _mmDeviceEnumerator.GetDefaultAudioEndpoint(DataFlow.Render, Role.Multimedia);
            }
            catch (Exception)
            {
                defaultDevice = null;
            }
           
            var devices = _mmDeviceEnumerator.EnumerateAudioEndPoints(DataFlow.Render, DeviceState.Active);
            
            if (defaultDevice == null || devices.Count == 0)
            {
	            return null;
            }

            MMDevice device;
            if (_preferences.SoundOutDeviceId == DefaultDevicePlaceholder)
            {
                device = defaultDevice;
            }
            else
            {
                device = devices.FirstOrDefault(x => x.ID == _preferences.SoundOutDeviceId);

                if (device == null)
                {
                    _preferences.SoundOutDeviceId = DefaultDevicePlaceholder;
                    return GetAudioOutput();
                }
            }
            _currentDeviceId = device.ID;
            if(device.State == DeviceState.Active)
            {
                //return new NAudio.Wave.WasapiOut(true, _preferences.AudioClientShareMode, _preferences.Latency) { Device = device, StreamRoutingOptions = StreamRoutingOptions.OnDeviceDisconnect | StreamRoutingOptions.OnFormatChange };
                return new WasapiOut(device, _preferences.AudioClientShareMode, true , _preferences.Latency);
            }

            Logging.Error("Specified audio device is not active.");

            return null;
            
            
        }

        private void RefreshAudioOutputProxy()
        {
            var result = new ObservableCollection<AudioDevice>();
           
            MMDevice standardDevice;
            try
            {
                standardDevice = _mmDeviceEnumerator.GetDefaultAudioEndpoint(DataFlow.Render, Role.Multimedia);
            }
            catch (Exception)
            {
                standardDevice = null;
            }

            if (IsWasapiSupportedOnCurrentPlatform)
            {
                var devices = _mmDeviceEnumerator.EnumerateAudioEndPoints(DataFlow.Render, DeviceState.Active);
                
                foreach (
                    var device in
                        devices.Select(
                            device =>
                                new AudioDevice(device.FriendlyName, device.ID, 
                                    standardDevice != null && standardDevice.ID == device.ID)))
                {
                    result.Add(device);
                }
                
                CheckDefaultAudioDevice(result);

            }
            
            AudioOutputList = result;
        }

		/// <summary>
		///     Gets a value which indicates whether Wasapi is supported on the current Platform. True means that the current
		///     platform supports <see cref="WasapiOut" />; False means that the current platform does not support
		///     <see cref="WasapiOut" />.
		/// </summary>
		public static bool IsWasapiSupportedOnCurrentPlatform
		{
			get { return Environment.OSVersion.Version.Major >= 6; }
		}

		private static void CheckDefaultAudioDevice(ObservableCollection<AudioDevice> devices)
        {
            if (devices.All(x => x.Id == DefaultDevicePlaceholder))
            {
	            devices.Clear(); //No default item if there are no other devices
            }
            else if (devices.Count > 0 &&
                     devices.All(x => x.Id != DefaultDevicePlaceholder))
            {
	            devices.Insert(0, new AudioDevice("Windows Default", "-0"));
            }
        }

        private void UpdateDefaultAudioDevice()
        {
            MMDevice defaultAudioEndpoint;
            defaultAudioEndpoint = _mmDeviceEnumerator.GetDefaultAudioEndpoint(DataFlow.Render, Role.Multimedia);
            
            AudioOutputList.ToList().ForEach(x => x.IsDefault = false);
	        var targetDevice = AudioOutputList.FirstOrDefault(x => x.Id == defaultAudioEndpoint.ID);
	        if (targetDevice != null) targetDevice.IsDefault = true;
        }

        
        private void CheckCurrentState()
        {
            if (AudioOutputList.Any(x => x.Id != DefaultDevicePlaceholder))
            {
                if (_enabled == true) return;
                _enabled = true;
                Enable?.Invoke(this, EventArgs.Empty);
                return;
            }

            if (_enabled == true)
            {
                _enabled = false;
                Disable?.Invoke(this, EventArgs.Empty);
            }
        }

        private void _mmNotificationClient_DefaultDeviceChanged(object sender, DefaultDeviceChangedEventArgs e)
        {
            if (e.DeviceId == _lastDefaultDeviceChanged) return;
            _lastDefaultDeviceChanged = e.DeviceId;
            if (_preferences.SoundOutDeviceId == DefaultDevicePlaceholder &&
                e.DeviceId == _currentDeviceId)
            {
                OnRefreshSoundOut();
            }

            UpdateDefaultAudioDevice();
        }

        private void _mmNotificationClient_DeviceStateChanged(object sender, DeviceStateChangedEventArgs e)
        {
            RefreshAudioOutputProxy();
            if (_currentDeviceId == e.DeviceId && e.DeviceState != DeviceState.Active)
            {
                OnRefreshSoundOut();
            }
        }

        private void _mmNotificationClient_DeviceRemoved(object sender, DeviceNotificationEventArgs e)
        {
	        RefreshAudioOutputProxy();
	        if (_currentDeviceId == e.DeviceId)
	        {
		        OnRefreshSoundOut();
	        }
        }

        private void _mmNotificationClient_DeviceAdded(object sender, DeviceNotificationEventArgs e)
        {
	        RefreshAudioOutputProxy();
        }

        private void SystemEvents_PowerModeChanged(object sender, PowerModeChangedEventArgs e)
        {
	        RefreshAudioOutputProxy();
        }
    }
}
