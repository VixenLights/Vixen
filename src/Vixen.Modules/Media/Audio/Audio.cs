﻿using Common.AudioPlayer;
using Vixen.Module;
using Vixen.Module.Media;
using Vixen.Module.Timing;
using VixenModules.Media.Audio.SampleProviders;

namespace VixenModules.Media.Audio
{
	public class Audio : MediaModuleInstanceBase, ITiming
	{
		private static NLog.Logger Logging = NLog.LogManager.GetCurrentClassLogger();
		private IPlayer _audioSystem;
		private AudioData _data;
		private CachedAudioData _cachedAudioData;
		
		[Obsolete("Based on old libraries and not currently implemented.")]
		public string[] DetectionNotes
		{
			get
			{
				return Array.Empty<string>();
				//if (_audioSystem == null) return null;
				//return _audioSystem.NOTE;
			}
		}
		
		[Obsolete("Based on old libraries and not currently implemented.")]
		public float[] DetectionNoteFreq
		{
			get
			{
				return Array.Empty<float>();
			//	if (_audioSystem == null) return null;
			//	return _audioSystem.NOTE_FREQ;
			//
			}
		}

		public bool MediaLoaded
		{
			get { return _audioSystem != null; }
		}

		[Obsolete("Based on old libraries and not currently implemented.")]
		public bool DetectFrequeniesEnabled
		{
			get
			{
				return false;
				//if (_audioSystem == null) return false;
				//else return _audioSystem.DetectFrequeniesEnabled;
			}
			set
			{
				//if (_audioSystem != null)
				//	_audioSystem.DetectFrequeniesEnabled = value;
			}
		}

		[Obsolete("Based on old libraries and not currently implemented.")]
		public bool LowPassFilterEnabled
		{
			get
			{
				return false;
				//if (_audioSystem == null) return false;
				//else return _audioSystem.LowPassFilterEnabled;
			}
			set
			{
				//if (_audioSystem != null)
				//	_audioSystem.LowPassFilterEnabled = value;
			}
		}

		[Obsolete("Based on old libraries and not currently implemented.")]
		public float LowPassFilterValue
		{
			get
			{
				return -1;
				//if (_audioSystem == null) return -1;
				//else return _audioSystem.LowPassFilterValue;
			}
			set { /*_audioSystem.LowPassFilterValue = value;*/ }
		}

		[Obsolete("Based on old libraries and not currently implemented.")]
		public float HighPassFilterValue
		{
			get
			{
				return -1;
				//if (_audioSystem == null) return -1;
				//else return _audioSystem.HighPassFilterValue;
			}
			set { /*_audioSystem.HighPassFilterValue = value;*/ }
		}

		[Obsolete("Based on old libraries and not currently implemented.")]
		public bool HighPassFilterEnabled
		{
			get
			{
				return false;
				//if (_audioSystem == null) return false;
				//else return _audioSystem.HighPassFilterEnabled;
			}
			set
			{
				//if (_audioSystem != null)
				//	_audioSystem.HighPassFilterEnabled = value;
			}
		}

		/// <summary>
		/// Number of bytes of data each sample contains.
		/// </summary>
		public int BytesPerSample
		{
			get
			{
				if (_audioSystem != null) {
					return _audioSystem.BytesPerSample;
				}

				return 0;
			}
		}

		/// <summary>
		/// The sample rate of the track.
		/// </summary>
		public float Frequency
		{
			get
			{
				if (_audioSystem != null) {
					return _audioSystem.Frequency;
				}

				return 0;
			}
		}

		/// <summary>
		/// Number of channels the audio track has.
		/// </summary>
		public int Channels
		{
			get
			{
				if (_audioSystem != null) {
					return _audioSystem.Channels;
				}

				return 0;
			}
		}

		public long NumberSamples {
			get
			{
				if (_audioSystem != null) {
					return _audioSystem.NumberSamples;
				}
				return 0;
			}
		}

		private void InitSampleProvider()
		{
			_cachedAudioData = new CachedAudioData(MediaFilePath);
		}

		/// <summary>
		/// Provides raw -1 to 1 averaged samples for by taking the max value over the number of samples to create a max sample.
		/// </summary>
		/// <param name="samplesPerInterval"></param>
		/// <returns></returns>
		public List<Sample> GetSamples(int numSamples, CancellationToken ct)
		{
			List<Sample> pi = new List<Sample>(numSamples);
            if (_cachedAudioData == null && MediaLoaded)
            {
                InitSampleProvider();
            }
			var provider = new MaxPeakProvider();
			CachedSoundSampleProvider cad = new CachedSoundSampleProvider(_cachedAudioData);
			var samplesPerInterval = cad.Length / (double)numSamples;
			provider.Init(cad, (int)samplesPerInterval);
			while (pi.Count < numSamples && cad.Position < cad.Length)
			{
				// Were we canceled?
				ct.ThrowIfCancellationRequested();
				cad.Position = (int)(samplesPerInterval * pi.Count);
				pi.Add(provider.GetNextPeak());
			}
			return pi;
		}

		/// <summary>
		/// Get the range of samples as a byte array from the starting sample. Converts -1 to 1 to byte values
		/// </summary>
		/// <param name="startSample">0 based starting sample</param>
		/// <param name="numSamples">Number of samples to include in the byte array</param>
		/// <returns></returns>
		public byte[] GetRawAudioSamples()
		{
			using (var audioFileReader = CSCore.Codecs.CodecFactory.Instance.GetCodec(_audioSystem.Filename))
			{
				
				var wholeFile = new List<byte>((int)(audioFileReader.Length / 4));
				var readBuffer = new byte[(int)(audioFileReader.WaveFormat.SampleRate * audioFileReader.WaveFormat.Channels)];
				int samplesRead;
				while ((samplesRead = audioFileReader.Read(readBuffer, 0, readBuffer.Length)) > 0)
				{
					wholeFile.AddRange(readBuffer.Take(samplesRead));
				}
				
				return wholeFile.ToArray();
			}
		}

		/// <summary>
		/// Gets the range of samples converted to a mono channel
		/// </summary>
		/// <param name="startSample"></param>
		/// <param name="numSamples"></param>
		/// <returns></returns>
		public double[] GetMonoSamples(int startSample, int numSamples)
		{	
			if (_cachedAudioData == null) return new double[0];
			CachedSoundSampleProvider cad = new CachedSoundSampleProvider(_cachedAudioData);
			var provider = new MonoSampleProvider(cad);
			cad.Position = startSample;
			var buffer = new double[numSamples];
			provider.Read(buffer, 0, numSamples);
			return buffer;
		}

		public override void Start()
		{
			if (_audioSystem != null && !_audioSystem.IsPlaying) {
				_audioSystem.Play(); 
			}
		}

		public override void Stop()
		{
			if (_audioSystem != null && _audioSystem.IsPlaying) {
				_audioSystem.Stop();
			}
		}

		public override void Pause()
		{
			if (_audioSystem != null && !_audioSystem.IsPaused) {
				_audioSystem.Pause();
			}
		}

		public override void Resume()
		{
			if (_audioSystem != null && _audioSystem.IsPaused) {
				_audioSystem.Resume();
			}
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				_DisposeAudio();		
			}
			base.Dispose(disposing);
		}

		private void _DisposeAudio()
		{
			if (_audioSystem != null)
			{
				_audioSystem.Stop();
				//_audioSystem.FrequencyDetected -= _audioSystem_FrequencyDetected;
				_audioSystem.Dispose();
				_audioSystem = null;
			}
		}

		public override ITiming TimingSource
		{
			get
			{
				if (_audioSystem != null && _audioSystem.AudioOutputManager.AudioOutputList.Any())
				{
					return this;
				}
				return null;
			}
		}

		public override IModuleDataModel ModuleData
		{
			get { return _data; }
			set { _data = value as AudioData; }
		}

		public override string MediaFilePath
		{
			get { return _data.FilePath; }
			set { _data.FilePath = value; }
		}

		public bool MediaExists
		{
			get { return File.Exists(MediaFilePath); } 
		}
		
		// If a media file is used as the timing source, it's also being
		// executed as media for the sequence.
		// That means we're either media or media and timing, so only
		// handle media execution entry points.
		public override void LoadMedia(TimeSpan startTime )
		{
			if (MediaLoaded)
			{
				_audioSystem.Position = startTime;
			}
			else
			{
				_DisposeAudio();
				if (File.Exists(MediaFilePath))
				{
					_audioSystem = PlayerFactory.CreateNew(MediaFilePath);
					_audioSystem.Position = startTime;
					InitSampleProvider();
				} 
				else
				{
					Logging.Error("Media file does not exist: " + MediaFilePath);
				}	
			}
			
		}

		public AudioOutputManager AudioOutputManager => _audioSystem.AudioOutputManager;

		/// <inheritdoc />
		[Obsolete("No longer populated and will be zero. Use CurrentPlaybackDeviceId")]
		public override int CurrentPlaybackDeviceIndex { get; set; }

		//public delegate void FrequencyDetectedHandler(object sender, FrequencyEventArgs e);

		//public event FrequencyDetectedHandler FrequencyDetected;

		//private void _audioSystem_FrequencyDetected(object sender, FrequencyEventArgs e)
		//{
		//	if (FrequencyDetected != null) {
		//		FrequencyDetected(this, e);
		//	}
		//}

		public TimeSpan Position
		{
			get
			{
				if (_audioSystem != null) {
					return _audioSystem.Position;
				}
				return TimeSpan.Zero;
			}
			set
			{
				if (_audioSystem != null)
				{
					_audioSystem.Position = value;
				}
			}
		}

		public TimeSpan MediaDuration
		{
			get
			{
				if (_audioSystem == null)
					LoadMedia(TimeSpan.Zero);

				return _audioSystem.Duration;
			}
		}

		public bool SupportsVariableSpeeds
		{
			get { return true; }
		}

		public float Speed
		{
			get => _audioSystem.PlaybackRate;
			set => _audioSystem.PlaybackRate = value;
		}

		public bool UseTempo
		{
			get => _audioSystem.UseTempo;
			set => _audioSystem.UseTempo = value;
		}
	}
}