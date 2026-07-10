namespace Vixen.IO.Policy
{
	internal abstract class ElementNodeTemplateFilePolicy : IFilePolicy
	{
		public void Write()
		{
			WriteElementNode();
		}

		protected abstract void WriteElementNode();

		public void Read()
		{
			ReadElementNode();
		}

		protected abstract void ReadElementNode();
	}
}