using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using CSCore.CoreAudioAPI;
using CSCore.DirectSound;
using CSCore.SoundOut;
using Microsoft.Win32;
using NLog;

namespace Common.AudioPlayer
{
	public class AudioOutputManager : IDisposable
    {
	    private static Logger Logging = LogManager.GetCurrentClassLogger();
	    private static AudioOutputManager _instance;
        public const string DefaultDevicePlaceholder = "-0";
        
        private string _currentDeviceId;

        private bool? _enabled;

        private string _lastDefaultDeviceChanged;
        private AudioPreferences _preferences;
        private readonly MMDeviceEnumerator _mmDeviceEnumerator;
        private readonly MMNotificationClient _mmNotificationClient;

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

        public ISoundOut GetAudioOutput()
        {
            MMDevice defaultDevice;
            
            try
            {
                defaultDevice = _mmDeviceEnumerator.GetDefaultAudioEndpoint(DataFlow.Render, Role.Multimedia);
            }
            catch (CoreAudioAPIException)
            {
                defaultDevice = null;
            }
            
            using (var devices = _mmDeviceEnumerator.EnumAudioEndpoints(DataFlow.Render, DeviceState.Active))
            {
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
                    device = devices.FirstOrDefault(x => x.DeviceID == _preferences.SoundOutDeviceId);

                    if (device == null)
                    {
                        _preferences.SoundOutDeviceId = DefaultDevicePlaceholder;
                        return GetAudioOutput();
                    }
                }
                _currentDeviceId = device.DeviceID;
                return new WasapiOut(true, AudioClientShareMode.Shared, _preferences.Latency) {Device = device, StreamRoutingOptions = StreamRoutingOptions.OnDeviceDisconnect|StreamRoutingOptions.OnFormatChange};
            }
            
        }

        private void RefreshAudioOutputProxy()
        {
            var result = new ObservableCollection<AudioDevice>();
           
            MMDevice standardDevice;
            try
            {
                standardDevice = _mmDeviceEnumerator.GetDefaultAudioEndpoint(DataFlow.Render, Role.Multimedia);
            }
            catch (CoreAudioAPIException)
            {
                standardDevice = null;
            }

            if (WasapiOut.IsSupportedOnCurrentPlatform)
            {
                using (var devices = _mmDeviceEnumerator.EnumAudioEndpoints(DataFlow.Render, DeviceState.Active))
                {
                    foreach (
                        var device in
                            devices.Select(
                                device =>
                                    new AudioDevice(device.FriendlyName, device.DeviceID, 
                                        standardDevice != null && standardDevice.DeviceID == device.DeviceID)))
                    {
                        result.Add(device);
                    }
                }

                CheckDefaultAudioDevice(result);

            }
            
            AudioOutputList = result;
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
	        var targetDevice = AudioOutputList.FirstOrDefault(x => x.Id == defaultAudioEndpoint.DeviceID);
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
