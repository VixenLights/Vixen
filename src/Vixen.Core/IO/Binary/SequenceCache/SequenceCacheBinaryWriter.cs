#nullable enable

using System.Runtime.Serialization;
using NLog;
using Vixen.Cache.Sequence;
using Vixen.IO.Loader;

namespace Vixen.IO.Binary.SequenceCache
{
	internal class SequenceCacheBinaryWriter : IObjectContentWriter
	{
		private static readonly Logger Logging = LogManager.GetCurrentClassLogger();
		public void WriteContentToObject(object content, object obj)
		{
			if (!(content is byte[] bytes)) throw new InvalidOperationException("Content must be a byte[].");
			if (!(obj is CacheContainer container)) throw new InvalidOperationException("Object must be a CacheContainer.");

			WriteContentToObject(bytes, container);
		}

		public void WriteContentToObject(byte[] content, CacheContainer cacheContainer)
		{
			
			using(Stream stream = new MemoryStream(content))
			{
				
				try
				{
					var reader = new DataContractSerializer(typeof(ISequenceCache));
					Object? obj = reader.ReadObject(stream);
					cacheContainer.SequenceCache = obj as ISequenceCache;
				}
				catch (Exception e)
				{
					Logging.Error(e, "Error deserializing cache instance");	
				}
				
				
			}

		}

		public int GetContentVersion(object content)
		{
			throw new NotImplementedException();
		}
	}
}
