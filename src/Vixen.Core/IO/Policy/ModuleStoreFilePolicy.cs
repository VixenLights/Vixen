namespace Vixen.IO.Policy
{
	internal abstract class ModuleStoreFilePolicy : IFilePolicy
	{
		public void Write()
		{
			WriteModuleTypeDataSet();
			WriteModuleInstanceDataSet();
		}

		protected abstract void WriteModuleTypeDataSet();
		protected abstract void WriteModuleInstanceDataSet();

		public void Read()
		{
			ReadModuleTypeDataSet();
			ReadModuleInstanceDataSet();
		}

		protected abstract void ReadModuleTypeDataSet();
		protected abstract void ReadModuleInstanceDataSet();
	}
}