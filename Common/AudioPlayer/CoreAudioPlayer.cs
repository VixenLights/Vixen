using System;
using System.Collections.Generic;
using Common.AudioPlayer.SampleProvider;
using CSCore;
using CSCore.CoreAudioAPI;
using CSCore.SoundOut;
using NLog;

namespace Common.AudioPlayer
{
	public class CoreAudioPlayer:IPlayer
	{
		private static Logger Logging = LogManager.GetCurrentClassLogger();
		private MMDevice _currentDevice;
		private ISoundOut _soundOut;
		private IWaveSource _waveSource;
		private int _latency = 25;
		private SoundTouchSource _speedControl;

		private static readonly MMDeviceEnumerator DeviceEnumerator;
		
		static CoreAudioPlayer()
		{
			DeviceEnumerator = new MMDeviceEnumerator();
		}
		
		public CoreAudioPlayer(string fileName):this(null, fileName)
		{
			
		}

		public CoreAudioPlayer(Device device, string fileName)
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

			if (mmDevice != null && mmDevice.DeviceState == DeviceState.Active)
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
				_waveSource?.Dispose();
				_waveSource = null;
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
				_waveSource =
					CSCore.Codecs.CodecFactory.Instance.GetCodec(fileName)
						.ToSampleSource()
						.AppendSource(x => new SoundTouchSource(x, 20), out _speedControl)
						.ToWaveSource();
				_soundOut.Initialize(_waveSource);
				status = true;
			}
			catch (Exception e)
			{
				Logging.Error(e, $"An error occurred opening the file. {fileName}");
				CloseFile();
			}

			return status;
		}

		public PlaybackState PlaybackState
		{
			get
			{
				if (_soundOut != null)
					return _soundOut.PlaybackState;
				return PlaybackState.Stopped;
			}
		}

		public static Device GetDeviceOrDefault(string id)
		{
			var mmDevice = DeviceEnumerator.GetDevice(id);
			var dd = GetDefaultDevice();
			if (mmDevice != null)
			{
				return new Device(mmDevice.FriendlyName, mmDevice.DeviceID, mmDevice.DeviceID == dd.Id);
			}

			return dd;
		}

		public static Device GetDefaultDevice()
		{
			var mmDevice = GetDefaultMMDevice();
			return new Device(mmDevice.FriendlyName, mmDevice.DeviceID, true);
		}

		private static MMDevice GetDefaultMMDevice()
		{
			var deviceEnumerator = new MMDeviceEnumerator();
			return deviceEnumerator.GetDefaultAudioEndpoint(DataFlow.Render, Role.Multimedia);
		}

		public static List<Device> GetActiveDevices()
		{
			var deviceEnumerator = new MMDeviceEnumerator();
			var mmDevices = deviceEnumerator.EnumAudioEndpoints(DataFlow.Render, DeviceState.Active);
			var defaultDevice = deviceEnumerator.GetDefaultAudioEndpoint(DataFlow.Render, Role.Multimedia);
			var devices = new List<Device>();
			foreach (var device in mmDevices)
			{
				devices.Add(new Device(device.FriendlyName, device.DeviceID ,device.DeviceID == defaultDevice.DeviceID));
			}

			return devices;
		}

		#region Implementation of IPlayer

		/// <inheritdoc />
		
        
		public bool IsPaused => PlaybackState == PlaybackState.Paused;
        
		/// <inheritdoc />
		public bool IsStopped => PlaybackState == PlaybackState.Stopped;

		/// <inheritdoc />
		public bool IsPlaying => PlaybackState == PlaybackState.Playing;

		/// <inheritdoc />
		public string Filename { get; }

		/// <inheritdoc />
		public TimeSpan Duration { get
		{
			if (_waveSource != null)
				return _waveSource.GetLength();
			return TimeSpan.Zero;
		} }

		/// <inheritdoc />
		public void Stop()
		{
			_soundOut?.Stop();
			PlaybackStopped?.Invoke();
		}

		/// <inheritdoc />
		public void Play()
		{
			_soundOut?.Play();
			PlaybackResumed?.Invoke();
		}

		/// <inheritdoc />
		public void Pause()
		{
			_soundOut?.Pause();
			PlaybackPaused?.Invoke();
		}

		/// <inheritdoc />
		public bool Resume()
		{
			var resumed = false;
			if (PlaybackState == PlaybackState.Paused)
			{
				Play();
				resumed = true;
			}

			return resumed;
		}

		/// <inheritdoc />
		public TimeSpan Position
		{
			get
			{
				if (_waveSource != null)
					return _waveSource.GetPosition();
				return TimeSpan.Zero;
			}
			set
			{
				_waveSource?.SetPosition(value);
			}
		}

		/// <inheritdoc />
		public int BytesPerSample => _waveSource.WaveFormat.BitsPerSample / 8;

		/// <inheritdoc />
		public long NumberSamples => _waveSource.Length / BytesPerSample;

		/// <inheritdoc />
		public int Channels => _waveSource.WaveFormat.Channels;

		/// <inheritdoc />
		public float Frequency  => _waveSource.WaveFormat.SampleRate;

		/// <inheritdoc />
		public float PlaybackRate
		{
			get
			{
				if (UseTempo)
				{
					if (_speedControl != null)
					{
						return _speedControl.Tempo;
					}
				}
				else
				{
					if (_speedControl != null)
					{
						return _speedControl.Rate;
					}
				}

				return 1;

			}
			set
			{
				if (UseTempo)
				{
					if (_speedControl != null)
					{
						_speedControl.Rate = 1.0f;
						_speedControl.Tempo = value;
					}
				}
				else
				{
					if (_speedControl != null)
					{ 
						_speedControl.Tempo = 1.0f;
						_speedControl.Rate = value;
					}
				}
			}
		}

		/// <inheritdoc />
		public bool UseTempo { get; set; }

		/// <inheritdoc />
		public float Volume
		{
			get
			{
				if (_soundOut != null)
					return _soundOut.Volume;
				return 1;
			}
			set
			{
				if (_soundOut != null)
				{
					_soundOut.Volume = Math.Min(1.0f, Math.Max(value, 0f));
				}
			}
		}

		/// <inheritdoc />
		public void SwitchAudioDevice(string mediaDeviceId)
		{
			var mmDevice = DeviceEnumerator.GetDevice(mediaDeviceId);
			
			if (mmDevice != null && mmDevice.DeviceState == DeviceState.Active)
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
				Stop();
			}
			
			if (resume)
			{
				Play();
			}
		}

		/// <inheritdoc />
		public string CurrentAudioDeviceId { get; private set; }

		/// <inheritdoc />
		public event Action PlaybackEnded;
		public event Action PlaybackResumed;
		public event Action PlaybackStopped;
		public event Action PlaybackPaused;

		#endregion

		private void CleanupPlayback()
		{
			if (_soundOut != null)
			{
				_soundOut.Dispose();
				_soundOut = null;
			}
			if (_waveSource != null)
			{
				_waveSource.Dispose();
				_waveSource = null;
			}
		}

		protected MMDevice CurrentDevice
		{
			get => _currentDevice;
			set
			{
				_currentDevice = value;
				CurrentAudioDeviceId = _currentDevice.DeviceID;
			}
		}

		private void EnsureDeviceCreated()
		{
			if (_soundOut == null)
			{
				CreateDevice();
			}
		}

		private void CreateDevice()
		{
			_soundOut = new WasapiOut{Latency = _latency, Device = CurrentDevice};
			if (PlaybackStopped != null) _soundOut.Stopped += PlaybackDeviceOnPlaybackStopped;
		}

		private void PlaybackDeviceOnPlaybackStopped(object sender, StoppedEventArgs e)
		{
			PlaybackEnded?.Invoke();
		}

		#region Implementation of IDisposable

		/// <inheritdoc />
		public void Dispose()
		{
			CleanupPlayback();
		}

		#endregion
	}
}
