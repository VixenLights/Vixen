using System;
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
		
		internal CoreAudioPlayer(string fileName):this(null, fileName)
		{
			
		}

		internal CoreAudioPlayer(Device device, string fileName)
		{
			if (string.IsNullOrEmpty(fileName))
			{
				throw new ArgumentNullException(nameof(fileName));
			}

			MMDevice mmDevice = null;
			if (device != null)
			{
				mmDevice = AudioDevices.GetDevice(device.Id);
			}

			if (mmDevice != null && mmDevice.DeviceState == DeviceState.Active)
			{
				CurrentDevice = mmDevice;
			}
			else
			{
				CurrentDevice = mmDevice ?? AudioDevices.GetDefaultAudioEndpoint();
			}

			Filename = fileName;
			Volume = 1.0f;
			Load();
		}

		public bool Load()
		{
			Stop();
			CloseFile();
			//EnsureDeviceCreated();
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
				//InitializeSound();
				status = true;
			}
			catch (Exception e)
			{
				Logging.Error(e, $"An error occurred opening the file. {fileName}");
				CloseFile();
			}

			return status;
		}

		private void InitializeSound()
		{
			try
			{
				_soundOut.Initialize(_waveSource);
				_soundOut.Volume = Volume;
			}
			catch (Exception e)
			{
				Logging.Error(e, $"An error occurred initializing the sound out device.");
			}
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
			if (IsStopped && EnsureDeviceCreated())
			{
				_soundOut?.Play();
				PlaybackResumed?.Invoke();
			}
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
				try
				{
					_waveSource?.SetPosition(value);
				}
				catch (Exception e)
				{
					Logging.Error(e, "Error setting position of wavestream.");
				}
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
		public bool UseTempo { get; set; } = false;

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
			if (CurrentAudioDeviceId == mediaDeviceId) return;
			var mmDevice = AudioDevices.GetDevice(mediaDeviceId);
			
			if (mmDevice != null && mmDevice.DeviceState == DeviceState.Active)
			{
				CurrentDevice = mmDevice;
			}
			else
			{
				CurrentDevice = mmDevice ?? AudioDevices.GetDefaultAudioEndpoint();
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
			CleanupSoundOut();
			if (_waveSource != null)
			{
				_waveSource.Dispose();
				_waveSource = null;
			}
		}

		private void CleanupSoundOut(bool dispose=true)
		{
			if (_soundOut != null)
			{
				_soundOut.Stopped -= PlaybackDeviceOnPlaybackStopped;
				if (dispose)
				{
					_soundOut.Dispose();
				}
				_soundOut = null;
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

		private bool EnsureDeviceCreated()
		{
			if (_soundOut == null)
			{
				return CreateDevice();
			}

			return true;
		}

		private bool CreateDevice()
		{
			if(CurrentDevice == null || CurrentDevice.DeviceState != DeviceState.Active)
			{
				var d = AudioDevices.GetDefaultDevice();
				if (d != null)
				{
					SwitchAudioDevice(d.Id);
				}
				else
				{
					return false;
				}
			};

			if (WasapiOut.IsSupportedOnCurrentPlatform)
				_soundOut = new WasapiOut{Latency = _latency, Device = CurrentDevice};
			else
			{
				Logging.Warn("Wasapi audio not supported. Defaulting to DirectSound on the default device.");
				_soundOut = new DirectSoundOut{Latency = _latency};
			}

			_soundOut.Stopped += PlaybackDeviceOnPlaybackStopped;
			if (_waveSource != null)
			{
				_soundOut.Initialize(_waveSource);
				_soundOut.Volume = Volume;
			}

			return true;
		}

		private void PlaybackDeviceOnPlaybackStopped(object sender, StoppedEventArgs e)
		{
			PlaybackEnded?.Invoke();
			CleanupSoundOut();
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
