using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Timers;
using FMOD;

namespace VixenModules.Media.Audio
{
	public partial class FmodInstance : IDisposable
	{
		private static NLog.Logger Logging = NLog.LogManager.GetCurrentClassLogger();

		private fmod _audioSystem;
		private FMOD.DSP dsplowpass = null;
		private FMOD.DSP dsphighpass = null;
		private FMOD.DSPConnection dspconnectiontemp = null;

		private SoundChannel _channel;
		private TimeSpan _startTime;
		private uint lengthPcmBytes;
		private int channels = 0;
		private int bits = 0;
		private float freq;

		private static bool lowPassFilterEnabled = false;
		private static bool highPassFilterEnabled = false;

		private Timer fmodUpdateTimer;
		
		public List<Tuple<int, string>> AudioDevices
		{
			get
			{
				List<Tuple<int, string>> ret = new List<Tuple<int, string>>();
				int i = 0;
				fmod.GetSoundDeviceList().ToList().ForEach(device => {
					ret.Add(new Tuple<int, string>(i, device));
					i++;
				});
				return ret;
			}
		}

		private static object lockObject = new object();
		public FmodInstance(string fileName=null)
		{
			lock (lockObject) {
				_audioSystem = fmod.GetInstance(fileName==null?-1:/*Vixen.Sys.State.Variables.SelectedAudioDeviceIndex*/0);
				if (_audioSystem == null || _audioSystem.SystemObject==null) return;
				_audioSystem.SystemObject.createDSPByType(FMOD.DSP_TYPE.LOWPASS, ref dsplowpass);
				_audioSystem.SystemObject.createDSPByType(FMOD.DSP_TYPE.HIGHPASS, ref dsphighpass);
			}
			//Load(fileName);
			//Changed from the above to allow sampling of the data. Could not extract data for waveform from the default
			//Is there another way to get the data with the default wrapper method.
			if (!string.IsNullOrWhiteSpace(fileName)) {
				LoadAsSample(fileName);
				populateStats();
			}
			fmodUpdateTimer = new Timer();
			fmodUpdateTimer.Elapsed += fmodUpdateTimer_Elapsed;
			fmodUpdateTimer.Interval = 100;
			fmodUpdateTimer.Enabled = true;
		}
		public int AudioDeviceIndex { get { return _audioSystem.DeviceIndex; } set { _audioSystem.DeviceIndex = value; } }
		private void fmodUpdateTimer_Elapsed(object sender, ElapsedEventArgs e)
		{
			lock (lockObject)
			{
				if (_audioSystem != null && _audioSystem.SystemObject != null)
					_audioSystem.SystemObject.update();	
			}	
			
		}

		public bool LowPassFilterEnabled
		{
			get { return lowPassFilterEnabled; }
			set
			{
				lowPassFilterEnabled = value;
				CheckIfFilterIsEnabled();
			}
		}


		public bool HighPassFilterEnabled
		{
			get { return highPassFilterEnabled; }
			set
			{
				highPassFilterEnabled = value;
				CheckIfFilterIsEnabled();
			}
		}

		public float LowPassFilterValue
		{
			get
			{
				float val = 0;
				StringBuilder sb = new StringBuilder();
				int len = 0;

				dsplowpass.getParameter(0, ref val, sb, len);
				return val;
			}
			set { dsplowpass.setParameter(0, value); }
		}

		public float HighPassFilterValue
		{
			get
			{
				float val = 0;
				StringBuilder sb = new StringBuilder();
				int len = 0;

				dsphighpass.getParameter(0, ref val, sb, len);
				return val;
			}
			set { dsphighpass.setParameter(0, value); }
		}

		private void CheckIfFilterIsEnabled()
		{
			bool active = false;

			dsphighpass.getActive(ref active);

			if (!highPassFilterEnabled && active)
				dsphighpass.remove();
			if (highPassFilterEnabled && !active)
				_audioSystem.SystemObject.addDSP(dsphighpass, ref dspconnectiontemp);

			active = false;
			dsplowpass.getActive(ref active);

			if (!lowPassFilterEnabled && active)
				dsplowpass.remove();
			if (lowPassFilterEnabled && !active)
				_audioSystem.SystemObject.addDSP(dsplowpass, ref dspconnectiontemp);
		}


		public byte[] GetSamples(int startSample, int numSamples)
		{
			int bufferSize = BytesPerSample*numSamples;
			int startByte = BytesPerSample*startSample;

			IntPtr ptr1 = IntPtr.Zero, ptr2 = IntPtr.Zero;
			uint len1 = 0, len2 = 0;

			checkErrors(_channel.Sound.@lock((uint) startByte, (uint) bufferSize, ref ptr1, ref ptr2, ref len1, ref len2));
			byte[] sampleBytes = new byte[len1 + len2];
			System.Runtime.InteropServices.Marshal.Copy(ptr1, sampleBytes, 0, (int) len1);
			if (len2 > 0) {
				System.Runtime.InteropServices.Marshal.Copy(ptr2, sampleBytes, (int)len1, (int) len2);
			}

			_channel.Sound.unlock(ptr1, ptr2, len1, len2);
			
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
			get { return lengthPcmBytes/BytesPerSample; }
		}

		public int BytesPerSample
		{
			get { return (bits/8)*channels; }
		}

		public float Frequency
		{
			get { return freq; }
		}

		public void LoadAsSample(string fileName)
		{
			//Adapted this code from the fmod wrappers loadsound method. Changed mode to CREATESAMPLE
			if (_channel != null && _channel.IsPlaying) {
				Stop();
			}
			if (_audioSystem == null) return;

			if (fileName == null || !File.Exists(fileName)) {
				return;
			}

			Sound sound = null;
			checkErrors(_audioSystem.SystemObject.createSound(fileName,
			                                                  (FMOD.MODE._2D | FMOD.MODE.HARDWARE | FMOD.MODE.CREATESAMPLE |
			                                                   MODE.ACCURATETIME), ref sound));
			if (_channel == null) {
				_channel = new SoundChannel(sound);
			}
			else {
				_channel.Sound = sound;
			}
		}

		private void checkErrors(FMOD.RESULT result)
		{
			if (result != FMOD.RESULT.OK) {
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
				Logging.Warn("Audio: can't load file '" + fileName + "' for playback. Does it exist?");
			}
			_startTime = TimeSpan.Zero;
		}

		public void SetStartTime(TimeSpan time)
		{
			_startTime = TimeSpan.Zero > time ? TimeSpan.Zero : time;
		}

		public void Play()
		{	
			AudioDeviceIndex=	0/*Vixen.Sys.State.Variables.SelectedAudioDeviceIndex*/;
			
			if (_channel != null && !_channel.IsPlaying) {
				SetStartTime(_startTime);
				_audioSystem.Play(_channel, true);
				_channel.Position = (uint) _startTime.TotalMilliseconds;
				_channel.Paused = false;
				IsFrequencyDetectionEnabled();
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

			set
			{
				if (_channel != null)
				{
					_channel.Position = (uint)value;
				} 
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

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (_audioSystem != null)
				{
					if (fmodUpdateTimer != null)
					{
						fmodUpdateTimer.Stop();
						fmodUpdateTimer.Elapsed -= fmodUpdateTimer_Elapsed;
						fmodUpdateTimer.Dispose();
					}
					//*** channel need to be disposed?  If so, then should reloading the channel
					//    cause an intermediate disposal?
					_audioSystem.Stop(_channel);

					if (_channel != null)
					{
						_audioSystem.ReleaseSound(_channel);
					}
					_audioSystem.Shutdown();
					_audioSystem = null;
				}
			}

			
			
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

		public float Speed
		{
			get { return _channel.Frequency; }
			set { _channel.Frequency = value; }
		}
	}
}