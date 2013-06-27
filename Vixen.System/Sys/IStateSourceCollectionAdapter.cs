using System.Collections.Generic;

namespace Vixen.Sys
{
	internal interface IStateSourceCollectionAdapter<K, out V> : IEnumerable<IStateSource<V>>
	{
		K Key { get; set; }
	}
}