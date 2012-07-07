using Vixen.Sys;

namespace Vixen.IO.Policy {
	public interface IFilePolicy : IVersioned {
		void Write();
		void Read();
	}
}
