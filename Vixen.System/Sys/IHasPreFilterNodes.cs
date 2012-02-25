using System.Collections.Generic;

namespace Vixen.Sys {
	public interface IHasPreFilterNodes {
		void AddPreFilter(PreFilterNode module);
		void AddPreFilters(IEnumerable<PreFilterNode> module);
		bool RemovePreFilter(PreFilterNode module);
		IEnumerable<PreFilterNode> GetAllPreFilters();
		void ClearPreFilters();
	}
}
