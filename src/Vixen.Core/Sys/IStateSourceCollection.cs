namespace Vixen.Sys
{
	internal interface IStateSourceCollection<in K, out V>
	{
		IStateSource<V> GetState(K key);
	}
}