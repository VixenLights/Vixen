using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using NAudio.CoreAudioApi;
using NAudio.CoreAudioApi.Interfaces;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using NAudioWrapper;
using NLog;

namespace Common.AudioPlayer
{
    public class AudioPlayback : IMMNotificationClient, IPlayer
    {
	    private static Logger Logging = LogManager.GetCurrentClassLogger();

        private IWavePlayer _playbackDevice;
        private WaveStream _fileStream;

        public event Action PlaybackResumed;
        public event Action PlaybackStopped;

        /// <inheritdoc />
        public byte[] GetSamples(int startSample, int numSamples)
        {
	        return new byte[0];
        }

        public event Action PlaybackEnded;
		public event Action PlaybackPaused;
		public event Action<VolumeEventArgs> OnPreVolumeMeter;
		public event Action<VolumeEventArgs> OnSteamVolume;
		public MMDevice CurrentDevice;
		private VolumeSampleProvider _volumeProvider;
		private float _volume = 1f;
		private static readonly MMDeviceEnumerator DeviceEnumerator;
		private AudioClientShareMode _audioClientShareMode = AudioClientShareMode.Shared;
		private int _latency = 20;

		static AudioPlayback()
		{
			DeviceEnumerator = new MMDeviceEnumerator();
		}

		public AudioPlayback(string fileName):this(null, fileName)
		{
			
		}

		public AudioPlayback(Device device, string fileName)
		{
			if (string.IsNullOrEmpty(fileName))
			{
				throw new ArgumentNullException(nameof(fileName));
			}

			MMDevice mmDevice = null;
			if (device != null)
			{
				mmDevice = DeviceEnumerator.GetDevice(device.Id);
			}

			if (mmDevice != null && mmDevice.State == DeviceState.Active)
			{
				CurrentDevice = mmDevice;
			}
			else
			{
				CurrentDevice = mmDevice ?? DeviceEnumerator.GetDefaultAudioEndpoint(DataFlow.Render, Role.Multimedia);
			}

			Filename = fileName;
			Load();
		}

		public static Device GetDeviceOrDefault(string id)
		{
			var mmDevice = DeviceEnumerator.GetDevice(id);
			var dd = GetDefaultDevice();
			if (mmDevice != null)
			{
				return new Device(mmDevice, mmDevice.ID == dd.Id);
			}

			return dd;
		}

		public static Device GetDefaultDevice()
		{
			var mmDevice = GetDefaultMMDevice();
			return new Device(mmDevice, true);
		}

		private static MMDevice GetDefaultMMDevice()
		{
			var deviceEnumerator = new MMDeviceEnumerator();
			return deviceEnumerator.GetDefaultAudioEndpoint(DataFlow.Render, Role.Multimedia);
		}

		public static List<Device> GetActiveDevices()
		{
			var deviceEnumerator = new MMDeviceEnumerator();
			var mmDevices = deviceEnumerator.EnumerateAudioEndPoints(DataFlow.Render, DeviceState.Active);
			var defaultDevice = deviceEnumerator.GetDefaultAudioEndpoint(DataFlow.Render, Role.Multimedia);
			var devices = new List<Device>();
			foreach (var device in mmDevices)
			{
				devices.Add(new Device(device,device.ID == defaultDevice.ID));
			}

			return devices;
		}

		/// <summary>
		/// Registers a call back for Device Events
		/// </summary>
		/// <param name="client">Object implementing IMMNotificationClient type casted as IMMNotificationClient interface</param>
		/// <returns></returns>
		public int RegisterEndpointNotificationCallback([In] [MarshalAs(UnmanagedType.Interface)] IMMNotificationClient client)
		{
			//DeviceEnum declared below
			return DeviceEnumerator.RegisterEndpointNotificationCallback(client);
		}

		/// <summary>
		/// UnRegisters a call back for Device Events
		/// </summary>
		/// <param name="client">Object implementing IMMNotificationClient type casted as IMMNotificationClient interface </param>
		/// <returns></returns>
		public int UnRegisterEndpointNotificationCallback([In] [MarshalAs(UnmanagedType.Interface)] IMMNotificationClient client)
		{
			//DeviceEnum declared below
			return DeviceEnumerator.UnregisterEndpointNotificationCallback(client);
		} 

		public bool Load()
        {
            Stop();
            CloseFile();
            EnsureDeviceCreated();
            return OpenFile(Filename);
        }
		
		private void CloseFile()
        {
	        try
	        {
		        _fileStream?.Dispose();
		        _fileStream = null;
	        }
	        catch (Exception e)
	        {
		        Logging.Error(e, "Unable to close the file stream.");
	        }
        }

        private bool OpenFile(string fileName)
        {
	        var status = false;
            try
            {
                var inputStream = new AudioFileReader(fileName);
				_fileStream = inputStream;
                Initialize(inputStream.ToSampleProvider());
                status = true;
            }
            catch (Exception e)
            {
	            Logging.Error(e, $"An error occurred opening the file. {fileName}");
                CloseFile();
            }

            return status;
        }

        private void Initialize(ISampleProvider inputStream)
        {
	        _volumeProvider = new VolumeSampleProvider(inputStream);
			//var postVolumeMeter = new MeteringSampleProvider(_volumeProvider);
			//postVolumeMeter.StreamVolume += PostVolumeMeter_StreamVolume;
			_playbackDevice.Init(_volumeProvider);
        }

		private void PostVolumeMeter_StreamVolume(object sender, StreamVolumeEventArgs e)
		{
			OnSteamVolume?.Invoke(new VolumeEventArgs(e.MaxSampleValues.Length > 0 ? e.MaxSampleValues[0] : 0, e.MaxSampleValues.Length> 1?e.MaxSampleValues[1]:0));
		}

		private void SampleChannelOnPreVolumeMeter(object sender, StreamVolumeEventArgs e)
        {
	        OnPreVolumeMeter?.Invoke(new VolumeEventArgs(e.MaxSampleValues[0], e.MaxSampleValues[1]));
        }

        private void EnsureDeviceCreated()
        {
            if (_playbackDevice == null)
            {
                CreateDevice();
            }
        }

        private void CreateDevice()
        {
            _playbackDevice = new WasapiOut(CurrentDevice,_audioClientShareMode, false, _latency);
			_playbackDevice.PlaybackStopped += PlaybackDeviceOnPlaybackStopped;
        }

        private void PlaybackDeviceOnPlaybackStopped(object sender, StoppedEventArgs e)
        {
	        //DisposePlaybackDevice();
			PlaybackEnded?.Invoke();
        }

        public void Play()
        {
			EnsureDeviceCreated();
            if (_fileStream != null && _playbackDevice.PlaybackState != PlaybackState.Playing)
            {
	            _volumeProvider.Volume = Volume;
                _playbackDevice.Play();
				PlaybackResumed?.Invoke();
            }
        }

        public void Pause()
        {
            _playbackDevice?.Pause();
			PlaybackPaused?.Invoke();
        }

        /// <inheritdoc />
        public string Filename { get; }

        public void Stop()
        {
			_playbackDevice?.Stop();
			//DisposePlaybackDevice();
            if (_fileStream != null)
            {
                _fileStream.Position = 0;
            }
			PlaybackStopped?.Invoke();
        }

        public bool IsPlaying => _playbackDevice?.PlaybackState == PlaybackState.Playing;
        
        public bool IsPaused => _playbackDevice?.PlaybackState == PlaybackState.Paused;
        
        public bool IsStopped => _playbackDevice?.PlaybackState == PlaybackState.Stopped;
        

        public bool Resume()
        {
	        var resumed = false;
	       if (_playbackDevice.PlaybackState == PlaybackState.Paused)
	        {
		        Play();
		        resumed = true;
	        }

	        return resumed;
        }

		public TimeSpan Position
        {
	        get => _fileStream?.CurrentTime ?? TimeSpan.Zero;
	        set
	        {
		        if (_fileStream != null)
		        {
			        _fileStream.CurrentTime = value;
		        }
			}
		}

		/// <inheritdoc />
		public int BytesPerSample => _fileStream.WaveFormat.BitsPerSample / 8;

		public long NumberSamples => _fileStream.Length / BytesPerSample;

		public int Channels => _fileStream.WaveFormat.Channels;

		public float Frequency => _fileStream.WaveFormat.SampleRate;

		public TimeSpan Duration => _fileStream.TotalTime;

		public float Volume
        {
	        get => _volume;

	        set
	        {
				EnsureDeviceCreated();
				_volume = Math.Min(1,Math.Max(0, value));
				if (_volumeProvider != null)
				{
					_volumeProvider.Volume = _volume;
				}
	        }
        }

		/// <inheritdoc />
		public void SetPlaybackSettings(int latency, bool eventMode, bool exclusiveMode)
		{
			_audioClientShareMode = exclusiveMode?AudioClientShareMode.Exclusive:AudioClientShareMode.Shared;
			_latency = latency;

		}

		public void Dispose()
        {
            Stop();
            CloseFile();
            DisposePlaybackDevice();
        }

		/// <inheritdoc />
		public string CurrentAudioDeviceId => CurrentDevice?.ID;

		/// <inheritdoc />
		public void SwitchAudioDevice(string mediaDeviceId)
		{
			var mmDevice = DeviceEnumerator.GetDevice(mediaDeviceId);
			
			if (mmDevice != null && mmDevice.State == DeviceState.Active)
			{
				CurrentDevice = mmDevice;
			}
			else
			{
				CurrentDevice = mmDevice ?? DeviceEnumerator.GetDefaultAudioEndpoint(DataFlow.Render, Role.Multimedia);
			}

			bool resume = false;
			if (IsPlaying || IsPaused)
			{
				resume = true;
				_playbackDevice?.Stop();
			}
			DisposePlaybackDevice();
			if (resume)
			{
				Play();
			}
		}

		private void DisposePlaybackDevice()
        {
	        _playbackDevice?.Dispose();
	        _playbackDevice = null;
		}

        #region Implementation of IMMNotificationClient

        /// <inheritdoc />
        public void OnDeviceStateChanged(string deviceId, DeviceState newState)
        {
	        
        }

        /// <inheritdoc />
        public void OnDeviceAdded(string pwstrDeviceId)
        {
	        
        }

        /// <inheritdoc />
        public void OnDeviceRemoved(string deviceId)
        {
	        
        }

        /// <inheritdoc />
        public void OnDefaultDeviceChanged(DataFlow flow, Role role, string defaultDeviceId)
        {
	        
        }

        /// <inheritdoc />
        public void OnPropertyValueChanged(string pwstrDeviceId, PropertyKey key)
        {
	        
        }

        #endregion
    }
}
