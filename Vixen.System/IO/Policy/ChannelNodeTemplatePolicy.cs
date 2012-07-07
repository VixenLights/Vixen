namespace Vixen.IO.Policy {
	abstract class ChannelNodeTemplatePolicy : IFilePolicy {
		public void Write() {
			WriteChannelNode();
		}

		abstract protected void WriteChannelNode();

		public void Read() {
			ReadChannelNode();
		}

		abstract protected void ReadChannelNode();

		public int Version {
			get { return 1; }
		}
	}
}
