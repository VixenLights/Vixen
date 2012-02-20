namespace Vixen.IO {
	abstract class ProgramFilePolicy : IFilePolicy {
		virtual public void Write() {
			WriteSequences();
		}

		abstract protected void WriteSequences();

		virtual public void Read() {
			ReadSequences();
		}

		abstract protected void ReadSequences();

		public int GetVersion() {
			return 1;
		}
	}
}
