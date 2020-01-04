using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Common.AudioPlayer;
using NAudio.Wave;
using Vixen.Module;
using Vixen.Module.Media;
using Vixen.Module.Timing;
using VixenModules.Media.Audio.SampleProviders;

namespace VixenModules.Media.Audio
{
	public class Audio : MediaModuleInstanceBase, ITiming
	{
		private static NLog.Logger Logging = NLog.LogManager.GetCurrentClassLogger();
		private AudioPlayback _audioSystem;
		private AudioData _data;
		private CachedAudioData _cachedAudioData;

		public string[] DetectionNotes
		{
			get
			{
				return Array.Empty<string>();
				//if (_audioSystem == null) return null;
				//return _audioSystem.NOTE;
			}
		}
		public override string CurrentPlaybackDeviceId
		{
			get
			{
				return Vixen.Sys.State.Variables.AudioDeviceId;
			}
			set
			{
				Vixen.Sys.State.Variables.AudioDeviceId = value;
			}
		}

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

		public List<Sample> GetSamples(int samplesPerInterval)
		{
			List<Sample> pi = new List<Sample>();
			if (_cachedAudioData == null) return pi;
			MaxPeakProvider provider = new MaxPeakProvider();
			CachedSoundSampleProvider cad = new CachedSoundSampleProvider(_cachedAudioData);
			provider.Init(cad, samplesPerInterval);
			while (cad.Position < cad.Length)
			{
				pi.Add(provider.GetNextPeak());
			}
			return pi;
		}

		/// <summary>
		/// Get the number samples as a byte array from the starting sample. 
		/// </summary>
		/// <param name="startSample">0 based starting sample</param>
		/// <param name="numSamples">Number of samples to include in the byte array</param>
		/// <returns></returns>
		public byte[] GetSamples(int startSample, int numSamples)
		{
			byte[] buffer;
			using (var reader = new AudioFileReader(MediaFilePath))
			{
				int bytesPerSample = (reader.WaveFormat.BitsPerSample / 8);
				var samples = reader.Length / (bytesPerSample);
				reader.Position = startSample * bytesPerSample;
				buffer = new byte[numSamples*bytesPerSample];
				var samplesRead = reader.Read(buffer, 0, buffer.Length);
				Console.Out.WriteLine($"Samples Read {samplesRead}");
			}

			return buffer;
		}

		public override void Start()
		{
			if (_audioSystem != null && !_audioSystem.IsPlaying) {
				if (_audioSystem.CurrentAudioDeviceId != CurrentPlaybackDeviceId)
				{
					_audioSystem.SwitchAudioDevice(CurrentPlaybackDeviceId);
				}
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
				if (_audioSystem != null && AudioPlayback.GetActiveDevices().Any())
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
		//public List<Tuple<int, string>> AudioDevices
		//{
		//	get
		//	{
		//		return _audioSystem.AudioDevices;
		//	}
		//}
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
					_audioSystem = new AudioPlayback(AudioPlayback.GetDeviceOrDefault(Vixen.Sys.State.Variables.AudioDeviceId), MediaFilePath);
					//_audioSystem.FrequencyDetected += _audioSystem_FrequencyDetected;
					_audioSystem.Position = startTime;
					InitSampleProvider();
				} else
				{
					Logging.Error("Media file does not exist: " + MediaFilePath);
				}	
			}
			
		}

		/// <inheritdoc />
		public override int CurrentPlaybackDeviceIndex { get; set; }

		public delegate void FrequencyDetectedHandler(object sender, FrequencyEventArgs e);

		public event FrequencyDetectedHandler FrequencyDetected;

		private void _audioSystem_FrequencyDetected(object sender, FrequencyEventArgs e)
		{
			if (FrequencyDetected != null) {
				FrequencyDetected(this, e);
			}
		}

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
			get { return _audioSystem.Frequency; }
			set { /*_audioSystem.Speed = value;*/ }
		}
	}
}