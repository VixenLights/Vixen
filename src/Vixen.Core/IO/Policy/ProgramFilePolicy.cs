namespace Vixen.IO.Policy
{
	internal abstract class ProgramFilePolicy : IFilePolicy
	{
		public virtual void Write()
		{
			WriteSequences();
		}

		protected abstract void WriteSequences();

		public virtual void Read()
		{
			ReadSequences();
		}

		protected abstract void ReadSequences();
	}
}