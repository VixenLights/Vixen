using System;

namespace Vixen.Sys {
	public interface IFilter {
		IFilterState CreateFilterState(TimeSpan filterRelativeTime);
	}
}
