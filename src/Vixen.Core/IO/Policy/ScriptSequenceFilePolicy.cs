namespace Vixen.IO.Policy {
	abstract class ScriptSequenceFilePolicy : IFilePolicy {
		public void Write() {
			WriteLanguage();
			WriteClassName();
			WriteSourceFileDirectory();
			WriteSourceFiles();
			WriteSourceFileContent();
			WriteFrameworkAssemblies();
			WriteExternalAssemblies();
		}

		protected abstract void WriteLanguage();
		protected abstract void WriteClassName();
		protected abstract void WriteSourceFileDirectory();
		protected abstract void WriteSourceFiles();
		protected abstract void WriteSourceFileContent();
		protected abstract void WriteFrameworkAssemblies();
		protected abstract void WriteExternalAssemblies();

		public void Read() {
			ReadLanguage();
			ReadClassName();
			ReadSourceFileDirectory();
			ReadSourceFiles();
			ReadSourceFileContent();
			ReadFrameworkAssemblies();
			ReadExternalAssemblies();
		}

		protected abstract void ReadLanguage();
		protected abstract void ReadClassName();
		protected abstract void ReadSourceFileDirectory();
		protected abstract void ReadSourceFiles();
		protected abstract void ReadSourceFileContent();
		protected abstract void ReadFrameworkAssemblies();
		protected abstract void ReadExternalAssemblies();

		public int GetVersion() {
			return 1;
		}
	}
}
