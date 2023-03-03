

using System.Diagnostics.CodeAnalysis;
using NAudio.Wave;

namespace Common.AudioPlayer.SampleProvider
{
    internal sealed class SoundTouchSource : ISampleProvider
    {
	    private readonly float[] _sourceReadBuffer;
        private readonly float[] _soundTouchReadBuffer;
        private readonly object _lockObject;
        private float _tempo = 1f;
        private float _rate = 1f;

        private bool _seekRequested;

        private ISampleProvider _sampleSource;
        private SoundTouch.SoundTouch _soundTouch;

        public SoundTouchSource(ISampleProvider sampleSource, int latency)
        {
	        _soundTouch = new SoundTouch.SoundTouch();
            // explore what the default values are before we change them:
            //Debug.WriteLine(String.Format("SoundTouch Version {0}", soundTouch.VersionString));
            //Debug.WriteLine("Use QuickSeek: {0}", soundTouch.GetUseQuickSeek());
            //Debug.WriteLine("Use AntiAliasing: {0}", soundTouch.GetUseAntiAliasing());

            _sampleSource = sampleSource;

            _soundTouch.SetChannels(_sampleSource.WaveFormat.Channels);
            _soundTouch.SetSampleRate(_sampleSource.WaveFormat.SampleRate);
            _soundTouch.SetUseAntiAliasing(false);
            _soundTouch.SetUseQuickSeek(false);
            _soundTouch.SetTempo(1.0f);

            _sourceReadBuffer = new float[(_sampleSource.WaveFormat.SampleRate * _sampleSource.WaveFormat.Channels * (long)latency) / 1000];
            _soundTouchReadBuffer = new float[_sourceReadBuffer.Length * 10];

            _lockObject = new object();
        }

        public void SetPitch(float pitch)
        {
            if (pitch > 6.0f || pitch < -6.0f)
            {
                pitch = 0.0f;
            }

            _soundTouch.SetPitchSemiTones(pitch);
        }

        public float Tempo
        {
            get => _tempo;

            set
            {
                _tempo = value == 0.0f ? 1f : value;
                _soundTouch.SetTempo(_tempo);
            }
        }

        public float Rate
        {
            get => _rate;

            set
            {
                _rate = value == 0.0f ? 1f : value;
                _soundTouch.SetRate(_rate);
            }
        }

        public void Seek()
        {
            _seekRequested = true;
        }

        [SuppressMessage("ReSharper", "CompareOfFloatsByEqualityOperator")]
        public int Read(float[] buffer, int offset, int count)
        {
	        if (_rate == 1.0f && _tempo == 1.0f)
            {
                return PassThroughRead(buffer, offset, count);
            }

            lock (_lockObject)
            {
                if (_seekRequested)
                {
                    _soundTouch.Clear();
                    _seekRequested = false;
                }

                var samplesRead = 0;
                var endOfSource = false;

                while (samplesRead < count)
                {
                    if (_soundTouch.NumberOfSamplesAvailable == 0)
                    {
                        var readFromSource = _sampleSource.Read(_sourceReadBuffer, 0, _sourceReadBuffer.Length);
                        if (readFromSource == 0)
                        {
                            endOfSource = true;
                            _soundTouch.Flush();
                        }

                        _soundTouch.PutSamples(_sourceReadBuffer, readFromSource / _sampleSource.WaveFormat.Channels);
                    }

                    var desiredSampleFrames = (count - samplesRead) / _sampleSource.WaveFormat.Channels;
                    var received = _soundTouch.ReceiveSamples(_soundTouchReadBuffer, desiredSampleFrames) * _sampleSource.WaveFormat.Channels;

                    for (int n = 0; n < received; n++)
                    {
                        buffer[offset + samplesRead++] = _soundTouchReadBuffer[n];
                    }

                    if (received == 0 && endOfSource)
                    {
                        break;
                    }
                }

                return samplesRead;
            }
        }

        /// <inheritdoc />
        public WaveFormat WaveFormat => _sampleSource.WaveFormat;

        public int PassThroughRead(float[] buffer, int offset, int count)
        {
            return _sampleSource.Read(buffer, offset, count);
        }

    }
}