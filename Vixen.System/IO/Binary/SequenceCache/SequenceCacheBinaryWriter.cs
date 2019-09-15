using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
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
			if (!(content is byte[])) throw new InvalidOperationException("Content must be a byte[].");
			if (!(obj is CacheContainer)) throw new InvalidOperationException("Object must be a CacheContainer.");

			WriteContentToObject((byte[])content, (CacheContainer)obj);
		}

		public void WriteContentToObject(byte[] content, CacheContainer cacheContainer)
		{
			
			using(Stream stream = new MemoryStream(content))
			{
				
				try
				{
					BinaryFormatter reader = new BinaryFormatter();
					Object obj = reader.Deserialize(stream);
					cacheContainer.SequenceCache = obj as ISequenceCache;
				}
				catch (Exception e)
				{
					Logging.Error(e, "Error deserializing cache instance");	
				}
				
				
			}

		}

		private void _ReadHeader(BinaryReader reader, ISequenceCache sequenceCache)
		{
			sequenceCache.SequenceFilePath = reader.ReadString();
			sequenceCache.Interval = reader.ReadInt32();
			sequenceCache.Length = TimeSpan.FromMilliseconds(reader.ReadDouble());
		}

		private int _getOutputCount(BinaryReader reader)
		{
			return reader.ReadInt32();
		}

		private int _getDataCount(BinaryReader reader)
		{
			return reader.ReadInt32();
		}

		private List<Guid> _ReadOutputs(BinaryReader reader, int outputCount)
		{
			var outputs = new List<Guid>();
			for (int i = 0; i < outputCount; i++)
			{
				var guid = new Guid(reader.ReadBytes(16));
				outputs.Add(guid);
			}

			return outputs;
		}

		private void _ReadData(BinaryReader reader, ISequenceCache sequenceCache, int dataCounts, int outputCounts)
		{
			
			for (int i = 0; i < dataCounts; i++)
			{
				//sequenceCache.AppendData(reader.ReadBytes(outputCounts).ToList());
			}	
		}

		public int GetContentVersion(object content)
		{
			throw new NotImplementedException();
		}
	}
}
