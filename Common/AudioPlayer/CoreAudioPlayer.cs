using System;
using Common.AudioPlayer.SampleProvider;
using CSCore;
using CSCore.SoundOut;
using NLog;

namespace Common.AudioPlayer
{
	public class CoreAudioPlayer:IPlayer
	{
		private static Logger Logging = LogManager.GetCurrentClassLogger();
		private ISoundOut _soundOut;
		private IWaveSource _waveSource;
		private SoundTouchSource _speedControl;
		
		internal CoreAudioPlayer(string fileName)
		{
			if (string.IsNullOrEmpty(fileName))
			{
				throw new ArgumentNullException(nameof(fileName));
			}

			AudioOutputManager = AudioOutputManager.Instance();
			
			Filename = fileName;
			Volume = 1.0f;
			Load();
		}

		private bool Load()
		{
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
				_speedControl?.Dispose();
				_waveSource?.Dispose();
				_waveSource =
					CSCore.Codecs.CodecFactory.Instance.GetCodec(fileName)
						.ToSampleSource()
						.AppendSource(x => new SoundTouchSource(x, 20), out _speedControl)
						.ToWaveSource();
				status = true;
			}
			catch (Exception e)
			{
				Logging.Error(e, $"An error occurred opening the file. {fileName}");
				CloseFile();
			}

			return status;
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

				try
				{
					if (_waveSource != null)
						return _waveSource.GetPosition();
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
		public event Action PlaybackEnded;
		public event Action PlaybackResumed;
		public event Action PlaybackStopped;
		public event Action PlaybackPaused;

		#endregion

		private void CleanupPlayback()
		{
			CleanUpWaveSource();
			CleanupSoundOut();
		}

		private void CleanUpWaveSource()
		{
			_speedControl?.Dispose();
			_waveSource?.Dispose();
			_waveSource = null;
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
				_soundOut.Stopped += PlaybackDeviceOnPlaybackStopped;
			}

			try
			{
				_soundOut.Initialize(_waveSource);
				_soundOut.Volume = Volume;
			}
			catch (Exception e)
			{
				Console.WriteLine(e);
				throw;
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
