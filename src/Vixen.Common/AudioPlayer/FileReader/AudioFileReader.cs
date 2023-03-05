using NAudio.Wave.SampleProviders;
using NAudio.Wave;

namespace Common.AudioPlayer.FileReader
{
    /// <summary>
    /// AudioFileReader simplifies opening an audio file in NAudio
    /// Simply pass in the filename, and it will attempt to open the
    /// file and set up a conversion path that turns into PCM IEEE float.
    /// ACM codecs will be used for conversion.
    /// It provides a volume property and implements both WaveStream and
    /// ISampleProvider, making it possibly the only stage in your audio
    /// pipeline necessary for simple playback scenarios
    /// </summary>
    public class AudioFileReader : WaveStream, ISampleProvider
    {
        private WaveStream _readerStream; // the waveStream which we will use for all positioning
        private readonly SampleChannel _sampleChannel; // sample provider that gives us most stuff we need
        private readonly int _destBytesPerSample;
        private readonly int _sourceBytesPerSample;
        private readonly long _length;
        private readonly object _lockObject;

        /// <summary>
        /// Initializes a new instance of AudioFileReader
        /// </summary>
        /// <param name="fileName">The file to open</param>
        public AudioFileReader(string fileName)
        {
            _lockObject = new object();
            FileName = fileName;
            CreateReaderStream(fileName);
            _sourceBytesPerSample = _readerStream.WaveFormat.BitsPerSample / 8 * _readerStream.WaveFormat.Channels;
            _sampleChannel = new SampleChannel(_readerStream, false);
            _destBytesPerSample = 4 * _sampleChannel.WaveFormat.Channels;
            _length = SourceToDest(_readerStream.Length);
        }

        /// <summary>
        /// Creates the reader stream, supporting all filetypes in the core NAudio library,
        /// and ensuring we are in PCM format
        /// </summary>
        /// <param name="fileName">File Name</param>
        private void CreateReaderStream(string fileName)
        {

            if (fileName.EndsWith(".wav", StringComparison.OrdinalIgnoreCase))
            {
                _readerStream = new WaveFileReader(fileName);
                if (_readerStream.WaveFormat.Encoding != WaveFormatEncoding.Pcm && _readerStream.WaveFormat.Encoding != WaveFormatEncoding.IeeeFloat)
                {
                    _readerStream = WaveFormatConversionStream.CreatePcmStream(_readerStream);
                    _readerStream = new BlockAlignReductionStream(_readerStream);
                }
            }
            else if (fileName.EndsWith(".mp3", StringComparison.OrdinalIgnoreCase))
            {
                _readerStream = new MediaFoundationReader(fileName);
            }
            else if (fileName.EndsWith(".aiff", StringComparison.OrdinalIgnoreCase) || fileName.EndsWith(".aif", StringComparison.OrdinalIgnoreCase))
            {
                _readerStream = new AiffFileReader(fileName);
            }
            else if(fileName.EndsWith(".ogg", StringComparison.OrdinalIgnoreCase))
            {
	            _readerStream = new NAudio.Vorbis.VorbisWaveReader(fileName);
            }
            else
            {
                // fall back to media foundation reader, see if that can play it
                _readerStream = new MediaFoundationReader(fileName);
            }
        }
        /// <summary>
        /// File Name
        /// </summary>
        public string FileName { get; }

        /// <summary>
        /// WaveFormat of this stream
        /// </summary>
        public override WaveFormat WaveFormat => _sampleChannel.WaveFormat;

        /// <summary>
        /// Length of this stream (in bytes)
        /// </summary>
        public override long Length => _length;

        public long StreamLength => _readerStream.Length;

        /// <summary>
        /// Position of this stream (in bytes)
        /// </summary>
        public override long Position
        {
            get { return SourceToDest(_readerStream.Position); }
            set { lock (_lockObject) { _readerStream.Position = DestToSource(value); } }
        }

        /// <summary>
        /// Reads from this wave stream
        /// </summary>
        /// <param name="buffer">Audio buffer</param>
        /// <param name="offset">Offset into buffer</param>
        /// <param name="count">Number of bytes required</param>
        /// <returns>Number of bytes read</returns>
        public int ReadRaw(byte[] buffer, int offset, int count)
        {
	        return _readerStream.Read(buffer, offset / 4, count);
        }

		/// <summary>
		/// Reads from this wave stream
		/// </summary>
		/// <param name="buffer">Audio buffer</param>
		/// <param name="offset">Offset into buffer</param>
		/// <param name="count">Number of bytes required</param>
		/// <returns>Number of bytes read</returns>
		public override int Read(byte[] buffer, int offset, int count)
        {
            var waveBuffer = new WaveBuffer(buffer);
            int samplesRequired = count / 4;
            int samplesRead = Read(waveBuffer.FloatBuffer, offset / 4, samplesRequired);
            return samplesRead * 4;
        }

        /// <summary>
        /// Reads audio from this sample provider
        /// </summary>
        /// <param name="buffer">Sample buffer</param>
        /// <param name="offset">Offset into sample buffer</param>
        /// <param name="count">Number of samples required</param>
        /// <returns>Number of samples read</returns>
        public int Read(float[] buffer, int offset, int count)
        {
            lock (_lockObject)
            {
                return _sampleChannel.Read(buffer, offset, count);
            }
        }

        /// <summary>
        /// Gets or Sets the Volume of this AudioFileReader. 1.0f is full volume
        /// </summary>
        public float Volume
        {
            get { return _sampleChannel.Volume; }
            set { _sampleChannel.Volume = value; }
        }

        /// <summary>
        /// Helper to convert source to dest bytes
        /// </summary>
        private long SourceToDest(long sourceBytes)
        {
            return _destBytesPerSample * (sourceBytes / _sourceBytesPerSample);
        }

        /// <summary>
        /// Helper to convert dest to source bytes
        /// </summary>
        private long DestToSource(long destBytes)
        {
            return _sourceBytesPerSample * (destBytes / _destBytesPerSample);
        }

        /// <summary>
        /// Disposes this AudioFileReader
        /// </summary>
        /// <param name="disposing">True if called from Dispose</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_readerStream != null)
                {
                    _readerStream.Dispose();
                    _readerStream = null;
                }
            }
            base.Dispose(disposing);
        }
    }
}
