using Vixen.Sys;

namespace Vixen.Rule {
	public interface IPatchingRule {
		string Description { get; }
		ControllerReference[] GenerateControllerReferences(int consecutiveApplicationCount);
	}
}
