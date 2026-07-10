namespace Vixen.IO.Policy
{
	internal abstract class OutputFilterTemplatePolicy : IFilePolicy
	{
		public void Write()
		{
			WriteModuleDataSet();
			WriteOutputFilterCollections();
		}

		protected abstract void WriteModuleDataSet();
		protected abstract void WriteOutputFilterCollections();

		public void Read()
		{
			ReadModuleDataSet();
			ReadOutputFilterCollections();
		}

		protected abstract void ReadModuleDataSet();
		protected abstract void ReadOutputFilterCollections();

		public int Version
		{
			get { return 1; }
		}
	}
}