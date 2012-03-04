namespace Vixen.IO.Policy {
	interface IFilePolicy {
		void Write();
		void Read();
		int GetVersion();
	}
}
