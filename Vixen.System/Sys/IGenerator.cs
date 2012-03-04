using Vixen.Commands;

namespace Vixen.Sys {
	public interface IGenerator {
		void GenerateCommand(ICombinator combinator);
		ICommand Value { get; }
	}
}
