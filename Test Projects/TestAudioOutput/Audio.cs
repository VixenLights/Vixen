using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Sys;
using Vixen.Common;
using CommandStandard;
using Vixen.Module;
using Vixen.Module.Media;
using Vixen.Module.Timing;

namespace TestAudioOutput {
    // Single-card implementation
    public class Audio : IMediaModuleInstance, ITiming {
        private FmodInstance _audioSystem;
		private AudioData _audioData;

        ////In response to a load command...
        //string filePath;
        //_channel = _audioSystem.LoadSound(filePath, _channel);
        //if(_channel != null)
        //{
        //}
        ////In response to a play command...
        //_audioSystem.Play(_channel);
        ////To play from a non-zero position...
        //_audioSystem.Play(_channel, true);
        //_channel.Position = 1111;
        //_channel.Paused = false;
        ////In response to a stop command...
        //_audioSystem.Stop(_channel);

        public void Start() {
            if(_audioSystem != null && !_audioSystem.IsPlaying) {
                _audioSystem.Play();
            }
        }

        public void Stop() {
            if(_audioSystem != null && _audioSystem.IsPlaying) {
				_DisposeAudio();
            }
        }

		public void Pause() {
			if(_audioSystem != null && !_audioSystem.IsPaused) {
				_audioSystem.Pause();
			}
		}

		public void Resume() {
			if(_audioSystem != null && _audioSystem.IsPaused) {
				_audioSystem.Resume();
			}
		}

		public Guid TypeId {
            get { return AudioModule._typeId; }
        }

        public Guid InstanceId { get; set; }

		public string TypeName { get; set; }

        public IModuleDataModel ModuleData {
			get { return _audioData; }
			set { _audioData = value as AudioData; }
        }

        public void Setup() {
			using(AudioSetup audioSetup = new AudioSetup(_audioData)) {
                audioSetup.ShowDialog();
            }
        }

        public void Dispose() {
			_DisposeAudio();
        }

		private void _DisposeAudio() {
			if(_audioSystem != null) {
				_audioSystem.Stop();
				_audioSystem.Dispose();
				_audioSystem = null;
			}
		}

		//public bool IsRunning {
		//    get { return _audioSystem != null && _audioSystem.IsRunning; }
		//}

		public ITiming TimingSource {
			get { return this as ITiming; }
		}

		public string MediaFilePath {
			get { return _audioData.FilePath; }
			set { _audioData.FilePath = value; }
		}

		// If a media file is used as the timing source, it's also being
		// executed as media for the sequence.
		// That means we're either media or media and timing, so only
		// handle media execution entry points.
		public void LoadMedia(long startTime) {
			_DisposeAudio();
			_audioSystem = new FmodInstance(MediaFilePath);
			_audioSystem.SetStartTime(startTime);
		}

		public long Position {
			get { return _audioSystem.Position; }
			set { }
		}

	}
}
