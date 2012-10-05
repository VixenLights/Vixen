using System;
using Vixen.IO.Factory;

namespace Vixen.IO.Persistor {
	using Vixen.Sys;

	class ProgramPersistor : IObjectPersistor<Program> {
		public void SaveToFile(Program obj, string filePath) {
			IFileWriter fileWriter = FileWriterFactory.CreateFileWriter();
			IObjectContentReader contentReader = ObjectContentReaderFactory.Instance.CreateProgramContentReader();
			ObjectPersistorService.Instance.SaveToFile(obj, fileWriter, contentReader, filePath);

			if(obj != null) obj.FilePath = filePath;
		}

		public void SaveToFile(object obj, string filePath) {
			if(!(obj is Program)) throw new InvalidOperationException("Object must be a ModuleStore.");
			SaveToFile((Program)obj, filePath);
		}
	}
}
