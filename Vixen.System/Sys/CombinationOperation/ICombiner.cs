namespace Vixen.Sys.CombinationOperation {
	public interface ICombiner {
	}

	public interface ICombiner<T> : ICombiner {
		T Combine(T value, IIntentState intentState);
	}
}
