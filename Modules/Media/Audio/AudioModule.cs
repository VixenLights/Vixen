using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Module.Media;
using Vixen.Module.Timing;
using FMOD;

namespace VixenModules.Media.Audio
{
	public class AudioModule : MediaModuleInstanceBase, ITiming
	{
		private FmodInstance _audioSystem;

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

		override public string MediaFilePath { get; set; }

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
	}


	public class FmodInstance : IDisposable
	{
		private fmod _audioSystem;
		private SoundChannel _channel;
		private TimeSpan _startTime;

		public FmodInstance(string fileName)
		{
			_audioSystem = fmod.GetInstance(-1);
			Load(fileName);
		}

		public void Load(string fileName)
		{
			if (_channel != null && _channel.IsPlaying) {
				Stop();
			}
			_channel = _audioSystem.LoadSound(fileName, _channel);
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
