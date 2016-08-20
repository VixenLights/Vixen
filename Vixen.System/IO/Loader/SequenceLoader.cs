using System;
using System.IO;
using Vixen.IO.Factory;
using Vixen.Module.SequenceType;
using Vixen.Services;
using Vixen.Sys;

namespace Vixen.IO.Loader
{
	internal class SequenceLoader : IObjectLoader
	{
		public ISequence LoadFromFile(string filePath)
		{
			if (filePath == null) throw new InvalidOperationException("Cannot load from a null file path.");
			if (!File.Exists(filePath)) throw new InvalidOperationException(string.Format("File does not exist at path {0}.", filePath));

			IFileReader fileReader = FileReaderFactory.Instance.CreateFileReader();
			if (fileReader == null) return null;

			IObjectContentWriter contentWriter = ObjectContentWriterFactory.Instance.CreateSequenceContentWriter(filePath);
			if (contentWriter == null) return null;

			IContentMigrator contentMigrator = ContentMigratorFactory.Instance.CreateSequenceContentMigrator(filePath);
			if (contentMigrator == null) return null;

			ISequenceTypeModuleInstance sequenceTypeModule = SequenceTypeService.Instance.CreateSequenceFactory(filePath);
			if (sequenceTypeModule == null) return null;

			ISequence sequence = sequenceTypeModule.CreateSequence();
			if (sequence == null) return null;

			sequence = MigratingObjectLoaderService.Instance.LoadFromFile(sequence, filePath, fileReader, contentWriter,
			                                                              contentMigrator, sequenceTypeModule.ObjectVersion);

			if (sequence != null) sequence.FilePath = filePath;

			return sequence;
		}

		object IObjectLoader.LoadFromFile(string filePath)
		{
			ISequenceTypeModuleInstance sequenceTypeModule = SequenceTypeService.Instance.CreateSequenceFactory(filePath);

			if (sequenceTypeModule == null) return null;

			if (sequenceTypeModule.IsCustomSequenceLoader) {
				return sequenceTypeModule.LoadSequenceFromFile(filePath);
			}

			return LoadFromFile(filePath);
		}
	}
}