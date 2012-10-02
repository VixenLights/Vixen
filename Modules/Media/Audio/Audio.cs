using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Vixen.Module;
using Vixen.Module.Media;
using Vixen.Module.Timing;
using FMOD;

namespace VixenModules.Media.Audio
{
	public class Audio : MediaModuleInstanceBase, ITiming
	{
		private FmodInstance _audioSystem;
		private AudioData _data;

		/// <summary>
		/// Number of bytes of data each sample contains.
		/// </summary>
		public int BytesPerSample
		{
			get
			{
				if (_audioSystem != null)
				{
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
				if (_audioSystem != null)
				{
					return _audioSystem.Frequency;
				}

				return 0;
			}
		}
		
		/// <summary>
		/// Number of samples the audio track has.
		/// </summary>
		public long NumberSamples
		{
			get
			{
				if (_audioSystem != null)
				{
					return _audioSystem.NumberSamples;
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
				if (_audioSystem != null)
				{
					return _audioSystem.Channels;
				}

				return 0;
			}
		}

		/// <summary>
		/// Get the number samples as a byte array from the starting sample. 
		/// </summary>
		/// <param name="startSample">0 based starting sample</param>
		/// <param name="numSamples">Number of samples to include in the byte array</param>
		/// <returns></returns>
		public byte[] GetSamples(int startSample, int numSamples)
		{
			
			if (_audioSystem != null)
			{
				return _audioSystem.GetSamples(startSample,numSamples);
			}else
			{
				return null;
			}

		}

		override public void Start()
		{
			if (_audioSystem != null && !_audioSystem.IsPlaying) {
				_audioSystem.Play();
			}
		}

		override public void Stop()
		{
			if (_audioSystem != null && _audioSystem.IsPlaying) {
				_audioSystem.Stop();
			}
		}

		override public void Pause()
		{
			if (_audioSystem != null && !_audioSystem.IsPaused) {
				_audioSystem.Pause();
			}
		}

		override public void Resume()
		{
			if (_audioSystem != null && _audioSystem.IsPaused) {
				_audioSystem.Resume();
			}
		}

		override public void Dispose()
		{
			_DisposeAudio();
		}

		private void _DisposeAudio()
		{
			if (_audioSystem != null) {
				_audioSystem.Stop();
				_audioSystem.Dispose();
				_audioSystem = null;
			}
		}

		override public ITiming TimingSource
		{
			get { return this as ITiming; }
		}

		override public IModuleDataModel ModuleData
		{
			get { return _data; }
			set { _data = value as AudioData; }
		}

		override public string MediaFilePath
		{
			get { return _data.FilePath; }
			set { _data.FilePath = value; }
		}

		// If a media file is used as the timing source, it's also being
		// executed as media for the sequence.
		// That means we're either media or media and timing, so only
		// handle media execution entry points.
		override public void LoadMedia(TimeSpan startTime)
		{
			_DisposeAudio();
			_audioSystem = new FmodInstance(MediaFilePath);
			_audioSystem.SetStartTime(startTime);
		}

		public TimeSpan Position
		{
			get { return TimeSpan.FromMilliseconds(_audioSystem.Position); }
			set { }
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

		public bool SupportsVariableSpeeds {
			get { return false; }
		}

		public float Speed {
			get { return 1; } // 1 = 100%
			set { throw new NotSupportedException(); }
		}
	}


	internal class FmodInstance : IDisposable
	{
		private fmod _audioSystem;
		private SoundChannel _channel;
		private TimeSpan _startTime;
		private uint lengthPcmBytes;
		private int channels = 0;
		private int bits = 0;
		private float freq;

		public FmodInstance(string fileName)
		{
			_audioSystem = fmod.GetInstance(-1);
			//Load(fileName);
			//Changed from the above to allow sampling of the data. Could not extract data for waveform from the default
			//Is there another way to get the data with the default wrapper method.
			LoadAsSample(fileName);
			populateStats();
		}

		public byte[] GetSamples(int startSample, int numSamples)
		{
			int bufferSize = BytesPerSample*numSamples;
			int startByte = BytesPerSample*startSample;

			IntPtr ptr1 = IntPtr.Zero, ptr2 = IntPtr.Zero;
			uint len1 = 0, len2 = 0;
			
			checkErrors(_channel.Sound.@lock((uint) startByte, (uint) bufferSize, ref ptr1, ref ptr2, ref len1, ref len2));
			byte[] sampleBytes1 = new byte[len1];
			byte[] sampleBytes2 = new byte[len2];
			System.Runtime.InteropServices.Marshal.Copy(ptr1, sampleBytes1, 0, (int)len1);
			if(len2>0)
			{
				System.Runtime.InteropServices.Marshal.Copy(ptr2, sampleBytes2, 0, (int)len2);	
			}
			
			_channel.Sound.unlock(ptr1, ptr2, len1, len2);
			byte[] sampleBytes = new byte[sampleBytes1.Length + sampleBytes2.Length];
			System.Buffer.BlockCopy(sampleBytes1, 0, sampleBytes, 0, sampleBytes1.Length);
			System.Buffer.BlockCopy(sampleBytes2, 0, sampleBytes, sampleBytes1.Length, sampleBytes2.Length);
			
			return sampleBytes;
		}

		private void populateStats()
		{
			float fZero = 0;
			int iZero = 0;
			SOUND_TYPE type = SOUND_TYPE.RAW;
			SOUND_FORMAT format = SOUND_FORMAT.NONE;
			_channel.Sound.getFormat(ref type, ref format, ref channels, ref bits);
			_channel.Sound.getLength(ref lengthPcmBytes, TIMEUNIT.PCMBYTES);
			_channel.Sound.getDefaults(ref freq, ref fZero, ref fZero, ref iZero);
			
		}

		public int Channels
		{
			get { return channels; }
			set { channels = value; }
		}

		public long NumberSamples
		{
			get { return lengthPcmBytes / BytesPerSample; }
		}
		public int BytesPerSample
		{
			get { return (bits / 8) * channels; }
		}
		public float Frequency
		{
			get { return freq; } 
		}

		public void LoadAsSample(string fileName)
		{
			//Adapted this code from the fmod wrappers loadsound method. Changed mode to CREATESAMPLE
			if (_channel != null && _channel.IsPlaying)
			{
				Stop();
			}
			if (_audioSystem == null) return;

			if (fileName == null || !File.Exists(fileName))
			{
				return;
			}

			Sound sound = null;
			checkErrors(_audioSystem.SystemObject.createSound(fileName, (FMOD.MODE._2D | FMOD.MODE.HARDWARE | FMOD.MODE.CREATESAMPLE | MODE.ACCURATETIME), ref sound));
			if (_channel == null)
			{
				_channel = new SoundChannel(sound);
				
			}else
			{
				_channel.Sound = sound;
			}	
		}

		private void checkErrors(FMOD.RESULT result)
		{
			if (result != FMOD.RESULT.OK)
			{
				throw new Exception(string.Format("Sound system error ({0})\n\n{1}", result, FMOD.Error.String(result)));
			}
		}

		public void Load(string fileName)
		{
			if (_channel != null && _channel.IsPlaying) {
				Stop();
			}
			_channel = _audioSystem.LoadSound(fileName, _channel);
			if (_channel == null) {
				Vixen.Sys.VixenSystem.Logging.Warning("Audio: can't load file '" + fileName + "' for playback. Does it exist?");
			}
			_startTime = TimeSpan.Zero;
		}

		public void SetStartTime(TimeSpan time)
		{
			_startTime = TimeSpan.Zero > time ? TimeSpan.Zero : time;
		}

		public void Play()
		{
			if (_channel != null && !_channel.IsPlaying) {
				SetStartTime(_startTime);
				_audioSystem.Play(_channel, true);
				_channel.Position = (uint)_startTime.TotalMilliseconds;
				_channel.Paused = false;
			}
		}

		public void Pause()
		{
			if (_channel != null && _channel.IsPlaying) {
				_channel.Paused = true;
			}
		}

		public void Resume()
		{
			if (_channel != null && _channel.Paused) {
				_channel.Paused = false;
			}
		}

		public void Stop()
		{
			if (_channel != null && _channel.IsPlaying) {
				_audioSystem.Stop(_channel);
			}
		}

		public long Position
		{
			get
			{
				if (_channel != null) {
					return _channel.Position;
				}
				return 0;
			}
		}

		public bool IsPlaying
		{
			get { return _channel != null && _channel.IsPlaying; }
		}
		public bool IsPaused
		{
			get { return _channel != null && _channel.Paused; }
		}

		~FmodInstance()
		{
			Dispose();
		}

		public void Dispose()
		{
			//*** channel need to be disposed?  If so, then should reloading the channel
			//    cause an intermediate disposal?
			_audioSystem.Stop(_channel);
			if (_channel != null) {
				_audioSystem.ReleaseSound(_channel);
			}
			_audioSystem.Shutdown();
			_audioSystem = null;

			GC.SuppressFinalize(this);
		}

		public TimeSpan Duration
		{
			get
			{
				if (_channel != null)
					return TimeSpan.FromMilliseconds(_channel.SoundLength);
				else
					return TimeSpan.Zero;
			}
		}
	}
}
