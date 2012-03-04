namespace Vixen.Sys {
	public interface IFilterState : ITypeAffector {
		// This is for both pre-filters and post-filters.  Only pre-filters are time-sensitive,
		// so we don't want any time value in this interface.
	}

	//interface IFilterState<T> : IFilterState {
	//    IFilter Filter { get; }
	//    TimeSpan RelativeTime { get; }
	//    T FilterIntent(IIntentState intentState); 
	//}
}
