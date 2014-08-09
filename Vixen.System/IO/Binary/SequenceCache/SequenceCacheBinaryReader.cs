using System;
using System.Collections.Generic;
using System.IO;
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
			byte[] cacheData = null;
			using (var writer = new BinaryWriter(new MemoryStream()))
			{
				try
				{
					_WriteHeader(writer, sequenceCache);
					_WriteOutputs(writer, sequenceCache.Outputs);
					_WriteData(writer, sequenceCache.RetrieveData());
				}
				catch (Exception e)
				{
					Logging.ErrorException("Error serializing cache instance", e);	
				}
				
				writer.BaseStream.Flush();
				cacheData = ((MemoryStream) writer.BaseStream).ToArray();

			}

			return cacheData;
		}

		private void _WriteHeader(BinaryWriter writer, ISequenceCache sequenceCache)
		{
			writer.Write(sequenceCache.SequenceFilePath);
			writer.Write(sequenceCache.Interval);
			writer.Write(sequenceCache.Length.TotalMilliseconds);
			writer.Write(sequenceCache.Outputs.Count);
			writer.Write(sequenceCache.RetrieveData().Count);
		}

		private void _WriteOutputs(BinaryWriter writer, IEnumerable<Guid> outputs)
		{
			foreach (var guid in outputs)
			{
				writer.Write(guid.ToByteArray());
			}
		}

		private void _WriteData(BinaryWriter writer, IEnumerable<List<byte>> data)
		{
			foreach (var dataSlice in data)
			{
				writer.Write(dataSlice.ToArray());
			}
		}
	}
}
