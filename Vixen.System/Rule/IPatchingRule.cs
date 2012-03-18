using System.Collections.Generic;
using Vixen.Sys;

namespace Vixen.Rule {
	public interface IPatchingRule {
		string Description { get; }
		//ControllerReference[] GenerateControllerReferences(int channelCount);
		IEnumerable<ControllerReferenceCollection> GenerateControllerReferenceCollections(int channelCount);
		int OutputCountToPatch { get; }
	}
}
