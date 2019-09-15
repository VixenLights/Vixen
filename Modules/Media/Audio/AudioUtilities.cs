using System;
using System.Threading.Tasks;
using NLog;

namespace VixenModules.Media.Audio
{
    public class AudioNotLoadedException : Exception
    {
    }

    public class AudioUtilities
    {
		private static readonly Logger Logging = LogManager.GetCurrentClassLogger();
		private Audio _audioModule;
        private float _audioSampleRate;
		private bool _audioLoaded;

		/// <summary>
		/// Calculated volume level at a given sample
		/// </summary>
		private double[] _volume;

        /// <summary>
        /// Gain to be applied to the volume.
        /// </summary>
        public double Gain { get; set; }

        /// <summary>
        /// Lowest volume in the loaded track
        /// </summary>
        private double _volumeFloor;

        /// <summary>
        /// Highest voluem in the loaded track
        /// </summary>
        private double _volumeCeiling;

        /// <summary>
        /// Lowest value in the unscaled volume waveform
        /// </summary>
        public double VolumeFloor { get { return _volumeFloor; } }

        /// <summary>
        /// Highest value in the unscaled volume waveform
        /// </summary>
        public double VolumeCeiling { get { return _volumeCeiling; } }

        /// <summary>
        /// Normalize the track so that it's peak is at 0 db before applying gain.
        /// </summary>
        public bool Normalize { get; set; }

        /// <summary>
        /// The raw audio data from the file
        /// </summary>
        //designed to have a range of +/- 1. The loading routine should determine the bitdepth
        //and do the appropriate scaling
        private double[] _audioChannel;

        /// <summary>
        /// The time, in ms, in which the volume is allowed to go up 30 db
        /// </summary>
        public int AttackTime { get; set; }

        /// <summary>
        /// The time, in ms, in which the volume is allowed to go down 30 db
        /// </summary>
        public int DecayTime { get; set; }

        /// <summary>
        /// Enables the low pass filter
        /// </summary>
        public bool LowPass { get; set; }

        /// <summary>
        /// THe frequency of the low pass filter
        /// </summary>
        public double LowPassFreq { get; set; }

        /// <summary>
        /// Enables the high pass filter
        /// </summary>
        public bool HighPass { get; set; }

        /// <summary>
        /// The frequency of the high pass filter
        /// </summary>
        public double HighPassFreq { get; set; }

		/// <summary>
		/// Duration of time to analyze
		/// </summary>
		public TimeSpan TimeSpan { get; set; }

		/// <summary>
		/// Start time into the media to analyze
		/// </summary>
		public TimeSpan StartTime { get; set; }

        /// <summary>
        /// Has audio been loaded and processed.
        /// </summary>
        public bool AudioLoaded { get { return _audioLoaded; } }

        public AudioUtilities()
        {
            _audioLoaded = false;
            Gain = 1;
            AttackTime = 300;
            DecayTime = 1000;
            Normalize = true;
            LowPass = false;
            LowPassFreq = 100;
        }

        /// <summary>
        /// The rate of the audio file in Hz
        /// </summary>
        public float AudioSampleRate { get { return _audioSampleRate; } }

        /// <summary>
        /// Find the audio module and load the audio as a mono channel into memory
        /// </summary>
        public bool ReloadAudio(Audio audio)
        {
	        if (audio == null)
	        {
				throw new ArgumentNullException("audio");
	        }
            
            if (audio.Channels == 0)
                return false;
            _audioModule = audio;
            _audioSampleRate = _audioModule.Frequency;
			LoadAudioIntoMemory();
            return true;
        }

        private static int Bytes2Int(byte b1, byte b2, byte b3)
        {
            int r = 0;
            byte b0 = 0xff;

            if ((b1 & 0x80) != 0) r |= b0 << 24;
            r |= b1 << 16;
            r |= b2 << 8;
            r |= b3;
            return r;
        }

        /// <summary>
        /// Load the audio file into memory and sum the channels to mono
        /// </summary>

        private void LoadAudioIntoMemory()
        {
            int startSample = (int)(_audioSampleRate * StartTime.TotalSeconds);
            int totalSamples = (int)(_audioSampleRate * TimeSpan.TotalSeconds);

            byte[] _audioRawData = _audioModule.GetSamples(startSample, totalSamples);
            _audioChannel = new double[_audioRawData.Length / _audioModule.BytesPerSample];

            int maxSize=1;

            //Support 8, 16 & 24 bit audio
            switch (_audioModule.BytesPerSample / _audioModule.Channels)
            {
                case 1: maxSize = 127; break;
                case 2: maxSize = 32767; break;
                case 3: maxSize = 8388607; break;
            }

            Parallel.For(0, _audioRawData.Length / _audioModule.BytesPerSample, x =>
                {
                    double audioSum = 0;
                    for (int i = 0; i < _audioModule.Channels; i++)
                    {
                        int pos = x * _audioModule.BytesPerSample + i * (_audioModule.BytesPerSample / _audioModule.Channels);
                        switch (_audioModule.BytesPerSample / _audioModule.Channels)
                        {
                            case 1: audioSum += (sbyte)_audioRawData[pos];
                                break;
                            case 2: audioSum += BitConverter.ToInt16(_audioRawData, pos );
                                break;
                            case 3: audioSum += Bytes2Int(_audioRawData[pos+2],_audioRawData[pos+1],_audioRawData[pos]);
                                break;
                        }
                    }
                    _audioChannel[x] = (double)(audioSum / _audioModule.Channels) / maxSize;
                    if (_audioChannel[x] == 0) _audioChannel[x] = .0001;
                });

            if (LowPass)
                _audioChannel = AudioFilters.LowPass(LowPassFreq, (int) _audioModule.Frequency, _audioChannel);
            if (HighPass)
                _audioChannel = AudioFilters.HighPass(HighPassFreq, (int)_audioModule.Frequency, _audioChannel);

            RecalculateVolume();
            _audioLoaded = true;
        }

        /// <summary>
        /// Reads the audio file and calculates
        /// the volume with the class meter ballistics
        /// </summary>
        public void RecalculateVolume()
        {
            _volume = new double[_audioChannel.Length];
            _volume[0] = 20.0 * (double)Math.Log10((double)Math.Abs(_audioChannel[0]));
            double maxAttack = 30.0 / ((double)AttackTime/1000 * _audioSampleRate);
            double maxDecay = 30.0 / ((double)DecayTime / 1000 * _audioSampleRate);
            double fastDecay = 30.0 / (10.0 / 1000 * _audioSampleRate);
            int maxVolumeIndex = 0;
            int minVolumeIndex = 0;

	        for (int i = 1; i < _volume.Length; i++)
            {
                //Rectify the signal and convert to db
                var newSample = 20.0 * (double)Math.Log10((double)Math.Abs(_audioChannel[i]));

                //fastDecay converts the AC signal into a DC representation of the volume
                if (_volume[i - 1] > newSample)
                {
                    if((_volume[i-1] - newSample) > (fastDecay))
                        newSample = _volume[i-1]-fastDecay;
                }
                _volume[i] = newSample;

                if (_volume[i - 1] < _volume[i]) //Going up!
                {
                    if ((_volume[i] - _volume[i - 1]) > maxAttack)
                        _volume[i] = _volume[i - 1] + maxAttack;
                }
                else //Going down! (or nowhere)
                {
                    if ((_volume[i - 1] - _volume[i]) > maxDecay)
                        _volume[i] = _volume[i - 1] - maxDecay;
                }
                if (_volume[i] > _volume[maxVolumeIndex])
                    maxVolumeIndex = i;
                if (_volume[i] < _volume[minVolumeIndex])
                    minVolumeIndex = i;
            }
            
            _volumeFloor = _volume[minVolumeIndex];
            _volumeCeiling = _volume[maxVolumeIndex];

            if(Normalize)
                for (int i = 0; i < _volume.Length; i++)
                {
                    _volume[i] = (_volume[i] + Gain + Math.Abs(_volumeCeiling));
                }
            else
                for (int i = 0; i < _volume.Length; i++)
                {
                    _volume[i] = (_volume[i] + Gain);
                }
        }

        /// <summary>
        /// Gets the closest sample # to given time in ms relative to the start of the effect.
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public int ClosestSample(int time)
        {
            if (!_audioLoaded)
                throw new AudioNotLoadedException();
            if (time < 0)
                return 0;
            if (time >= TimeSpan.TotalMilliseconds)
                return _volume.Length-1;
            else
                return (int)(time * _audioSampleRate / 1000.0);
        }

        /// <summary>
        /// returns the volume at a given time in ms relative to the start of the effect
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public double VolumeAtTime(int time)
        {
            if (!_audioLoaded)
                throw new AudioNotLoadedException();
            return _volume[ClosestSample(time)];
        }

        /// <summary>
        /// Minimizes memory usage, must call ReloadAudio again for changes.
        /// </summary>
        public void FreeMem()
        {
            _audioChannel = null;
            _volume = null;
            _audioLoaded = false;
        }

        /// <summary>
        /// Play audio throught the sound device starting at the beginning of the effect
        /// </summary>
        public void StartPlayback()
        {
            //_effectSequence.Stop();
            _audioModule.LoadMedia(StartTime);
            _audioModule.Stop();
            _audioModule.Start();
        }

        /// <summary>
        /// Stop audio playback
        /// </summary>
        public void StopPlayback()
        {
            _audioModule.Stop();
        }

        /// <summary>
        /// Get the current position of the playback engine in ms
        /// </summary>
        /// <returns></returns>
        public int GetCurrentPlaybackTime()
        {
            return (int)_audioModule.Position.TotalMilliseconds;
        }
    }
}