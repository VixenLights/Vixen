using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using NLog;
using Vixen.Cache.Sequence;
using System.IO;

namespace Vixen.IO.Binary.SequenceCache
{
	internal class SequenceCacheBinaryReader : IObjectContentReader
	{
		private static readonly Logger Logging = LogManager.GetCurrentClassLogger();
		public object ReadContentFromObject(object obj)
		{
			ISequenceCache sequenceCache = obj as ISequenceCache;
			if (sequenceCache == null) throw new InvalidOperationException("Object must be an ISequenceCache.");

			return _GenerateSequenceCacheDataContent(sequenceCache);
		}

		private byte[] _GenerateSequenceCacheDataContent(ISequenceCache sequenceCache)
		{
			byte[] cacheData;
			var writer = new DataContractSerializer(typeof(ISequenceCache));
			using (var stream = new MemoryStream())
			{
				try
				{
					writer.WriteObject(stream,sequenceCache);
				}
				catch (Exception e)
				{
					Logging.Error(e, "Error serializing cache instance");	
				}
				
				stream.Flush();
				cacheData = stream.ToArray();

			}

			return cacheData;
		}
	}
}
