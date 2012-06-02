using System.Collections.Generic;

namespace Vixen.Sys {
	interface IStateSourceCollectionAdapter<K, out V> : IEnumerable<IStateSource<V>> {
		K Key { get; set; }
	}
}
