namespace Vixen.Sys
{
	public interface IStateSource<out V>
	{
		V State { get; }
	}
}