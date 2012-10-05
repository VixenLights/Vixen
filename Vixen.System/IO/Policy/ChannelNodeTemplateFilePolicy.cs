namespace Vixen.IO.Policy {
	abstract class ChannelNodeTemplateFilePolicy : IFilePolicy {
		public void Write() {
			WriteChannelNode();
		}

		abstract protected void WriteChannelNode();

		public void Read() {
			ReadChannelNode();
		}

		abstract protected void ReadChannelNode();
	}
}
