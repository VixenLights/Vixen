namespace Vixen.IO {
	abstract class ScriptSequenceFilePolicy : IFilePolicy {
		public void Write() {
			WriteLanguage();
			WriteClassName();
			WriteSourceFiles();
			WriteFrameworkAssemblies();
			WriteExternalAssemblies();
		}

		protected abstract void WriteLanguage();
		protected abstract void WriteClassName();
		protected abstract void WriteSourceFiles();
		protected abstract void WriteFrameworkAssemblies();
		protected abstract void WriteExternalAssemblies();

		public void Read() {
			ReadLanguage();
			ReadClassName();
			ReadSourceFiles();
			ReadFrameworkAssemblies();
			ReadExternalAssemblies();
		}

		protected abstract void ReadLanguage();
		protected abstract void ReadClassName();
		protected abstract void ReadSourceFiles();
		protected abstract void ReadFrameworkAssemblies();
		protected abstract void ReadExternalAssemblies();

		public abstract int GetVersion();
	}
}
