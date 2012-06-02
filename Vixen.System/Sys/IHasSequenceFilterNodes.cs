using System.Collections.Generic;

namespace Vixen.Sys {
	public interface IHasSequenceFilterNodes {
		void AddSequenceFilter(ISequenceFilterNode sequenceFilterNode);
		void AddSequenceFilters(IEnumerable<ISequenceFilterNode> module);
		bool RemoveSequenceFilter(ISequenceFilterNode sequenceFilterNode);
		IEnumerable<ISequenceFilterNode> GetAllSequenceFilters();
		void ClearSequenceFilters();
	}
}
