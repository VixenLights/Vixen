using System;
using System.IO;
using Vixen.IO.Factory;
using Vixen.Module.SequenceType;
using Vixen.Services;
using Vixen.Sys;

namespace Vixen.IO.Loader {
	class SequenceLoader : IObjectLoader {
		public ISequence LoadFromFile(string filePath) {
			if(filePath == null) throw new InvalidOperationException("Cannot load from a null file path.");
			if(!File.Exists(filePath)) throw new InvalidOperationException("File does not exist.");

			IFileReader fileReader = FileReaderFactory.Instance.CreateFileReader();
			IObjectContentWriter contentWriter = ObjectContentWriterFactory.Instance.CreateSequenceContentWriter(filePath);
			IContentMigrator contentMigrator = ContentMigratorFactory.Instance.CreateSequenceContentMigrator(filePath);
			ISequenceTypeModuleInstance sequenceTypeModule = SequenceTypeService.Instance.CreateSequenceFactory(filePath);
			ISequence sequence = sequenceTypeModule.CreateSequence();
			sequence = MigratingObjectLoaderService.Instance.LoadFromFile(sequence, filePath, fileReader, contentWriter, contentMigrator, sequenceTypeModule.ClassVersion);

			if(sequence != null) sequence.FilePath = filePath;

			return sequence;
		}

		object IObjectLoader.LoadFromFile(string filePath) {
			return LoadFromFile(filePath);
		}
	}
}
