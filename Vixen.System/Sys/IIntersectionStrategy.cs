namespace Vixen.Sys {
	interface IIntersectionStrategy<T> {
		T GetIntersectionOf(T baseObject, T otherObject);
	}
}
