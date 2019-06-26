using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using NLog;
using Vixen.Cache.Sequence;

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
			var writer = new BinaryFormatter();
			using (var stream = new MemoryStream())
			{
				try
				{
					writer.Serialize(stream,sequenceCache);
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
