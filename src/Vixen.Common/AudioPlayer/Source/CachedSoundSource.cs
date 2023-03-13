using NAudio.Wave;

namespace Common.AudioPlayer.Source
{
	public sealed class CachedSoundSource : WaveStream, IDisposable
	{
		private readonly Stream _cache;

		public CachedSoundSource(WaveStream source)
		{
			if (source == null)
				throw new ArgumentNullException(nameof(source));
			if (source.Length > Int32.MaxValue)
				throw new ArgumentException("Length is of source is too large.");

			WaveFormat = source.WaveFormat;

			_cache = CreateStream();
			CacheSource(source);
		}

		/// <summary>
		/// Creates a stream to buffer data in.
		/// </summary>
		/// <returns>An empty stream to use as buffer.</returns>
		private Stream CreateStream()
		{
			return new MemoryStream() { Position = 0 };
		}

		private void CacheSource(WaveStream source)
		{
			
			int read;
			int count = (int)Math.Min(source.WaveFormat.AverageBytesPerSecond * 5, source.Length);
			byte[] buffer = new byte[count];

			long position = 0;
			if (source.CanSeek)
				position = source.Position;

			while ((read = source.Read(buffer, 0, count)) > 0)
			{
				_cache.Write(buffer, 0, read);
			}

			if (source.CanSeek)
			{
				source.Position = position;
				_cache.Position = source.Position;
			}
			else
			{
				_cache.Position = 0;
			}
		}



		#region Overrides of Stream

		
		/// <summary>
		///     Reads a sequence of bytes from the cache and advances the position within the cache by the
		///     number of bytes read.
		/// </summary>
		/// <param name="buffer">
		///     An array of bytes. When this method returns, the <paramref name="buffer" /> contains the specified
		///     byte array with the values between <paramref name="offset" /> and (<paramref name="offset" /> +
		///     <paramref name="count" /> - 1) replaced by the bytes read from the cache.
		/// </param>
		/// <param name="offset">
		///     The zero-based byte offset in the <paramref name="buffer" /> at which to begin storing the data
		///     read from the cache.
		/// </param>
		/// <param name="count">The maximum number of bytes to read from the cache.</param>
		/// <returns>The total number of bytes read into the <paramref name="buffer"/>.</returns>
		public override int Read(byte[] buffer, int offset, int count)
		{
			CheckForDisposed();

			return _cache.Read(buffer, offset, count);
		}

		#region Overrides of WaveStream

		/// <inheritdoc />
		public override WaveFormat WaveFormat { get; }

		#endregion

		/// <summary>
		/// Gets the amount of bytes stored in the cache.
		/// </summary>
		public override long Length => _cache.Length;

		/// <summary>
		/// Gets or sets the position.
		/// </summary>
		public override long Position
		{
			get => _cache.Position;
			set
			{
				CheckForDisposed();
				value -= (value % WaveFormat.BlockAlign);
				_cache.Position = value;
			}
		}

		#endregion

		private bool _disposed;

		/// <summary>
		/// Disposes the cache.
		/// </summary>
		public new void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		/// <summary>
		/// Disposes the internal used cache. 
		/// </summary>
		/// <param name="disposing"></param>
		private new void Dispose(bool disposing)
		{
			if (!_disposed)
			{
				if (disposing)
				{
					_cache.Dispose();
				}
			}
			_disposed = true;
		}

		/// <summary>
		/// Finalizes an instance of the <see cref="CachedSoundSource"/> class.
		/// </summary>
		~CachedSoundSource()
		{
			Dispose(false);
		}

		private void CheckForDisposed()
		{
			if (_disposed)
				throw new ObjectDisposedException("CachedSoundSource");
		}


	}
}
