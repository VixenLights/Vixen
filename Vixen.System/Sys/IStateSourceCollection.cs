namespace Vixen.Sys {
	interface IStateSourceCollection<in K, out V> {
		IStateSource<V> GetValue(K key);
	}
}
