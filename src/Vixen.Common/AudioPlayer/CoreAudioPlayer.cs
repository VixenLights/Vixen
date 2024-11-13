#nullable enable
using Common.AudioPlayer.FileReader;
using Common.AudioPlayer.SampleProvider;
using Common.AudioPlayer.Source;
using NAudio.Wave;
using NLog;

namespace Common.AudioPlayer
{
	public class CoreAudioPlayer:IPlayer
	{
		private static readonly Logger Logging = LogManager.GetCurrentClassLogger();
		private IWavePlayer? _soundOut;
		private IWaveProvider? _waveSource;
		private SoundTouchSource? _speedControl;
		private CachedSoundSource? _cachedSoundSource;

		internal CoreAudioPlayer(string fileName)
		{
			if (string.IsNullOrEmpty(fileName))
			{
				throw new ArgumentNullException(nameof(fileName));
			}

			AudioOutputManager = AudioOutputManager.Instance();
			
			Filename = fileName;
			Volume = 1.0f;
			if (Load() == false)
				throw new InvalidOperationException($"Cannot load {fileName}");
		}

		private bool Load()
		{
			return OpenFile(Filename);
		}
		
		private void CloseFile()
		{
			try
			{
				//_waveSource?.Dispose();
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
				//_speedControl?.Dispose();
				//_waveSource?.Dispose();
				_waveSource = GetCodec(Filename);
					status = true;
			}
			catch (Exception e)
			{
				Logging.Error(e, $"An error occurred opening the file. {fileName}");
				CloseFile();
			}

			return status;
		}

		private IWaveProvider GetCodec(string filename)
		{
			try
			{
				AudioFileReader reader = new AudioFileReader(filename);
				_cachedSoundSource = new CachedSoundSource(reader);
				_speedControl = new SoundTouchSource(_cachedSoundSource.ToSampleProvider(), 20);
				return _speedControl.ToWaveSourceProvider(_cachedSoundSource.WaveFormat.BitsPerSample);
			}
			catch (Exception e)
			{
				throw new InvalidOperationException(e.Message, e);
			}
		}

		public AudioOutputManager AudioOutputManager { get; }

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
			if (_cachedSoundSource != null)
				return _cachedSoundSource.TotalTime;
			return TimeSpan.Zero;
		} }

		/// <inheritdoc />
		public void Stop()
		{
			_soundOut?.Stop();
			PlaybackStopped?.Invoke();
			CleanupSoundOut();
		}

		/// <inheritdoc />
		public void Play()
		{
			// If the play back state is stopped then
			// make sure the sound out wave player has been properly disposed.
			if (PlaybackState == PlaybackState.Stopped)
			{
				CleanupSoundOut();
			}

			if ((IsPaused || IsStopped) && EnsureDeviceCreated())
			{
				if (_soundOut == null) return;
				if (IsPaused)
				{
					_soundOut?.Play();
				}
				else
				{
					try
					{
						_soundOut.Init(_waveSource);
						_soundOut.Volume = Volume;
					}
					catch (Exception e)
					{
						Logging.Error(e, "An exception occured trying to initialize the audio player.");
						return;
					}
					_soundOut?.Play();
				}
				
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

				try
				{
					if (PlaybackState == PlaybackState.Stopped)
					{
						return TimeSpan.Zero;
					}

					if (_cachedSoundSource != null)
					{
						return _cachedSoundSource.CurrentTime;
					}

				}
				catch (Exception e)
				{
					Logging.Error(e, "An error occured trying to get the position.");
					Stop();
					PlaybackEnded?.Invoke();
					CleanupSoundOut();
				}
				return TimeSpan.Zero;
			}
			set
			{
				try
				{
					if(_cachedSoundSource != null)
					{
						_cachedSoundSource.CurrentTime = value;
					}
					
				}
				catch (Exception e)
				{
					Logging.Error(e, "Error setting position of wavestream.");
				}
			}
		}

		/// <inheritdoc />
		public int BytesPerSample => _waveSource==null?0:_waveSource.WaveFormat.BitsPerSample / 8;

		/// <inheritdoc />
		public long NumberSamples => _cachedSoundSource == null ? 0 : _cachedSoundSource.Length / BytesPerSample;

		/// <inheritdoc />
		public int Channels => _waveSource == null ? 0 : _waveSource.WaveFormat.Channels;

		/// <inheritdoc />
		public float Frequency  => _waveSource == null ? 0 : _waveSource.WaveFormat.SampleRate;

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
		public event Action? PlaybackEnded;
		public event Action? PlaybackResumed;
		public event Action? PlaybackStopped;
		public event Action? PlaybackPaused;

		#endregion

		private void CleanupPlayback()
		{
			CleanUpWaveSource();
			CleanupSoundOut();
		}

		private void CleanUpWaveSource()
		{
			//_speedControl?.Dispose();
			
			_waveSource = null;
		}

		private object _soundOutLock = new();

		private void CleanupSoundOut(bool dispose=true)
		{
			lock (_soundOutLock)
			{
				if (_soundOut != null)
				{
					_soundOut.PlaybackStopped -= PlaybackDeviceOnPlaybackStopped;
					if (dispose)
					{
						_soundOut.Dispose();
					}
					_soundOut = null;
				}
			}
		}

		private bool EnsureDeviceCreated()
		{
			if (_soundOut == null || _waveSource == null)
			{
				return InitializeDevice();
			}

			return true;
		}

		private bool InitializeDevice()
		{
			if (_waveSource == null)
			{
				var status = Load();
				if (!status || _waveSource == null) return false;
			}

			if (_soundOut == null)
			{
				_soundOut = AudioOutputManager.GetAudioOutput();
				if (_soundOut == null) return false;
				_soundOut.PlaybackStopped += PlaybackDeviceOnPlaybackStopped;
			}
			
			return true;
		}

		private void PlaybackDeviceOnPlaybackStopped(object? sender, StoppedEventArgs e)
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
