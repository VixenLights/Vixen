using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FMOD;

namespace TestAudioOutput {
    class FmodInstance : IDisposable {
        private fmod _audioSystem;
        private SoundChannel _channel;
		private TimeSpan _startTime;

        public FmodInstance(string fileName) {
            _audioSystem = fmod.GetInstance(-1);
            Load(fileName);
        }

        //public AudioTimingSource CreateAudioInstance() {
        //}

        public void Load(string fileName) {
            if(_channel != null && _channel.IsPlaying) {
                Stop();
            }
            _channel = _audioSystem.LoadSound(fileName, _channel);
			_startTime = TimeSpan.Zero;
        }

		public void SetStartTime(TimeSpan time) {
			_startTime = TimeSpan.Zero > time ? TimeSpan.Zero : time;
        }

        public void Play() {
            //if(_channel != null) {
            //    if(_startTime == 0) {
            //        _audioSystem.Play(_channel);
            //    } else {
            //        _audioSystem.Play(_channel, true);
            //        _channel.Position = (uint)_startTime;
            //        _channel.Paused = false;
            //    }
            //}
            if(_channel != null && !_channel.IsPlaying) {
                SetStartTime(_startTime);
                _audioSystem.Play(_channel, true);
                _channel.Position = (uint)_startTime.TotalMilliseconds;
                _channel.Paused = false;
            }
        }

        public void Pause() {
            if(_channel != null && _channel.IsPlaying) {
                _channel.Paused = true;
            }
        }

        public void Resume() {
            if(_channel != null && _channel.Paused) {
                _channel.Paused = false;
            }
        }

        public void Stop() {
            if(_channel != null && _channel.IsPlaying) {
                _audioSystem.Stop(_channel);
            }
        }

		public long Position {
            get {
                if(_channel != null) {
                    //System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
                    //sw.Start();
                    ////return (int)_channel.Position;
                    //int position = (int)_channel.Position;
                    //sw.Stop();
                    //long ms = sw.ElapsedMilliseconds;
                    //return position;
                    return _channel.Position;
                }
                return 0;// -1;
            }
        }

		//public bool IsRunning {
		//    get { return _channel != null && _channel.IsPlaying; }
		//}
		public bool IsPlaying {
			get { return _channel != null && _channel.IsPlaying; }
		}
		public bool IsPaused {
			get { return _channel != null && _channel.Paused; }
		}

        ~FmodInstance() {
            Dispose();
        }

        public void Dispose() {
            //*** channel need to be disposed?  If so, then should reloading the channel
            //    cause an intermediate disposal?
            _audioSystem.Stop(_channel);
            if(_channel != null) {
                _audioSystem.ReleaseSound(_channel);
            }
            _audioSystem.Shutdown();
            _audioSystem = null;

            GC.SuppressFinalize(this);
        }
    }
}
