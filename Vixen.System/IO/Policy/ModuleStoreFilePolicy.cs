namespace Vixen.IO.Policy {
	abstract class ModuleStoreFilePolicy : IFilePolicy {
		public void Write() {
			WriteModuleTypeDataSet();
			WriteModuleInstanceDataSet();
		}

		abstract protected void WriteModuleTypeDataSet();
		abstract protected void WriteModuleInstanceDataSet();

		public void Read() {
			ReadModuleTypeDataSet();
			ReadModuleInstanceDataSet();
		}

		abstract protected void ReadModuleTypeDataSet();
		abstract protected void ReadModuleInstanceDataSet();
	}
}
