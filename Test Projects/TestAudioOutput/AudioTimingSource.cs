using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FMOD;

namespace TestAudioOutput {
    class AudioTimingSource : Vixen.Interface.ITimingSource {
        private SoundChannel _channel;

        public AudioTimingSource() {
            _channel = 
        }

        public int Position {
            get { throw new NotImplementedException(); }
        }

        public void Play() {
            throw new NotImplementedException();
        }

        public void Pause() {
            throw new NotImplementedException();
        }

        public void Resume() {
            throw new NotImplementedException();
        }

        public void Stop() {
            throw new NotImplementedException();
        }
    }
}
