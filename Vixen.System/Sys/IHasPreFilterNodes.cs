using System.Collections.Generic;

namespace Vixen.Sys {
	public interface IHasPreFilterNodes {
		void AddPreFilter(IPreFilterNode module);
		void AddPreFilters(IEnumerable<IPreFilterNode> module);
		bool RemovePreFilter(IPreFilterNode module);
		IEnumerable<IPreFilterNode> GetAllPreFilters();
		void ClearPreFilters();
	}
}
