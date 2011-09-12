using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Sys;
using CommandStandard;
using Vixen.Module;
using Vixen.Module.Media;
using Vixen.Module.Timing;

namespace TestAudioOutput {
    // Single-card implementation
    public class Audio : MediaModuleInstanceBase, ITiming {
        private FmodInstance _audioSystem;
		private AudioData _audioData;
		private AudioStaticData _audioStaticData;

        override public void Start() {
            if(_audioSystem != null && !_audioSystem.IsPlaying) {
                _audioSystem.Play();
            }
        }

		override public void Stop() {
            if(_audioSystem != null && _audioSystem.IsPlaying) {
				_DisposeAudio();
            }
        }

		override public void Pause() {
			if(_audioSystem != null && !_audioSystem.IsPaused) {
				_audioSystem.Pause();
			}
		}

		override public void Resume() {
			if(_audioSystem != null && _audioSystem.IsPaused) {
				_audioSystem.Resume();
			}
		}

		override public IModuleDataModel ModuleData {
			get { return _audioData; }
			set { _audioData = value as AudioData; }
        }

		public override IModuleDataModel StaticModuleData {
			get { return _audioStaticData; }
			set { 
				_audioStaticData = value as AudioStaticData;
				if(_audioStaticData.SomethingSpecial == 0) {
					_audioStaticData.SomethingSpecial = 1;
				}
			}
		}

		override public void Setup() {
			using(AudioSetup audioSetup = new AudioSetup(_audioData)) {
                audioSetup.ShowDialog();
            }
        }

		override public void Dispose() {
			_DisposeAudio();
        }

		private void _DisposeAudio() {
			if(_audioSystem != null) {
				_audioSystem.Stop();
				_audioSystem.Dispose();
				_audioSystem = null;
			}
		}

		override public ITiming TimingSource {
			get { return this as ITiming; }
		}

		override public string MediaFilePath {
			get { return _audioData.FilePath; }
			set { _audioData.FilePath = value; }
		}

		// If a media file is used as the timing source, it's also being
		// executed as media for the sequence.
		// That means we're either media or media and timing, so only
		// handle media execution entry points.
		override public void LoadMedia(TimeSpan startTime) {
			_DisposeAudio();
			_audioSystem = new FmodInstance(MediaFilePath);
			_audioSystem.SetStartTime(startTime);
		}

		public TimeSpan Position {
			get { return TimeSpan.FromMilliseconds(_audioSystem.Position); }
			set { }
		}
	}
}
