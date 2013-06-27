using System;
using Vixen.IO.Factory;
using Vixen.Sys;

namespace Vixen.IO.Persistor
{
	internal class SequencePersistor : IObjectPersistor
	{
		public void SaveToFile(ISequence obj, string filePath)
		{
			IFileWriter fileWriter = FileWriterFactory.CreateFileWriter();
			IObjectContentReader contentReader = ObjectContentReaderFactory.Instance.CreateSequenceContentReader(filePath);
			ObjectPersistorService.Instance.SaveToFile(obj, fileWriter, contentReader, filePath);

			if (obj != null) obj.FilePath = filePath;
		}

		public void SaveToFile(object obj, string filePath)
		{
			if (!(obj is ISequence)) throw new InvalidOperationException("Object must be an ISequence.");
			SaveToFile((ISequence) obj, filePath);
		}
	}
}