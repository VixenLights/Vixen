namespace Vixen.IO.Policy
{
	internal abstract class SystemContextFilePolicy : IFilePolicy
	{
		public void Write()
		{
			WriteSourceIdentity();
			WriteContextName();
			WriteContextDescription();
			WriteFiles();
		}

		protected abstract void WriteSourceIdentity();
		protected abstract void WriteContextName();
		protected abstract void WriteContextDescription();
		protected abstract void WriteFiles();

		public void Read()
		{
			ReadSourceIdentity();
			ReadContextName();
			ReadContextDescription();
			ReadFiles();
		}

		protected abstract void ReadSourceIdentity();
		protected abstract void ReadContextName();
		protected abstract void ReadContextDescription();
		protected abstract void ReadFiles();
	}
}