namespace Vixen.IO.Policy {
	abstract class ModuleStoreFilePolicy : IFilePolicy {
		public void Write() {
			WriteModuleDataSet();
		}

		abstract protected void WriteModuleDataSet();

		public void Read() {
			ReadModuleDataSet();
		}

		abstract protected void ReadModuleDataSet();

		public int Version {
			get { return 1; }
		}
	}
}
