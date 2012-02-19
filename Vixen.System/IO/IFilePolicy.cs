namespace Vixen.IO {
	interface IFilePolicy {
		void Write();
		void Read();
		int GetVersion();
	}
}
