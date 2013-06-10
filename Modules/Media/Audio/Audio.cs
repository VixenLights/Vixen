using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Vixen.Module;
using Vixen.Module.Media;
using Vixen.Module.Timing;
using FMOD;
using System.Timers;

namespace VixenModules.Media.Audio
{
    public class Audio : MediaModuleInstanceBase, ITiming
    {
        private FmodInstance _audioSystem;
        private AudioData _data;

        public bool MediaLoaded
        {
            get
            {
                return _audioSystem != null;
            }
        }

        public bool LowPassFilterEnabled
        {
            get
            {
                if (_audioSystem == null) return false;
                else return _audioSystem.LowPassFilterEnabled;
            }
            set
            {
                if (_audioSystem != null)
                    _audioSystem.LowPassFilterEnabled = value;
            }
        }
        public float LowPassFilterValue
        {
            get
            {
                if (_audioSystem == null) return -1;
                else return _audioSystem.LowPassFilterValue;
            }
            set
            {
                _audioSystem.LowPassFilterValue = value;
            }
        }
        public float HighPassFilterValue
        {
            get
            {
                if (_audioSystem == null) return -1;
                else return _audioSystem.HighPassFilterValue;
            }
            set { _audioSystem.HighPassFilterValue = value; }
        }
        public bool HighPassFilterEnabled
        {
            get
            {
                if (_audioSystem == null) return false;
                else return _audioSystem.HighPassFilterEnabled;
            }
            set
            {
                if (_audioSystem != null)
                    _audioSystem.HighPassFilterEnabled = value;
            }
        }

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
                return _audioSystem.GetSamples(startSample, numSamples);
            }
            else
            {
                return null;
            }

        }

        override public void Start()
        {
            if (_audioSystem != null && !_audioSystem.IsPlaying)
            {
                _audioSystem.Play();
            }
        }

        override public void Stop()
        {
            if (_audioSystem != null && _audioSystem.IsPlaying)
            {
                _audioSystem.Stop();
            }
        }

        override public void Pause()
        {
            if (_audioSystem != null && !_audioSystem.IsPaused)
            {
                _audioSystem.Pause();
            }
        }

        override public void Resume()
        {
            if (_audioSystem != null && _audioSystem.IsPaused)
            {
                _audioSystem.Resume();
            }
        }

        override public void Dispose()
        {
            _DisposeAudio();
        }

        private void _DisposeAudio()
        {
            if (_audioSystem != null)
            {
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
            if (File.Exists(MediaFilePath))
            {
                _audioSystem = new FmodInstance(MediaFilePath);
                _audioSystem.SetStartTime(startTime);
            }
            else
            {
                throw new FileNotFoundException("Media file doe not exist: " + MediaFilePath);
            }

        }

        public TimeSpan Position
        {
            get
            {
                if (_audioSystem != null)
                {
                    return TimeSpan.FromMilliseconds(_audioSystem.Position);
                }
                return TimeSpan.Zero;
            }
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

        public bool SupportsVariableSpeeds
        {
            get { return true; }
        }

        public float Speed
        {
            get { return _audioSystem.Speed; }
            set { _audioSystem.Speed = value; }
        }
    }


  
}
