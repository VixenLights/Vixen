using System;
using Vixen.IO.Factory;
using Vixen.Cache.Sequence;

namespace Vixen.IO.Persistor
{
	internal class SequenceCachePersister : IObjectPersistor
	{
		public void SaveToFile(object obj, string filePath)
		{
			if (!(obj is ISequenceCache)) throw new InvalidOperationException("Object must be an ISequenceCache.");
			SaveToFile((ISequenceCache)obj, filePath);
		}

		public void SaveToFile(ISequenceCache obj, string filePath)
		{
			IFileWriter fileWriter = FileWriterFactory.CreateBinaryFileWriter();
			IObjectContentReader contentReader = ObjectContentReaderFactory.Instance.CreateSequenceCacheContentReader(filePath);
			ObjectPersistorService.Instance.SaveToFile(obj, fileWriter, contentReader, filePath);
		}
	}
}
