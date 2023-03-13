namespace Vixen.Sys
{
	public interface IHasSetup
	{
		bool HasSetup { get; }
		bool Setup();
	}
}