namespace Vixen.Module.Trigger
{
	public interface ITrigger
	{
		ITriggerInput[] TriggerInputs { get; }
		void UpdateState();
	}
}