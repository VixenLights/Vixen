namespace Vixen.IO.Policy {
	abstract class ElementNodeTemplateFilePolicy : IFilePolicy {
		public void Write() {
			WriteElementNode();
		}

		abstract protected void WriteElementNode();

		public void Read() {
			ReadElementNode();
		}

		abstract protected void ReadElementNode();
	}
}
