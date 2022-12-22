namespace Vixen.Module.Trigger
{
	public interface ITriggerInput
	{
		TriggerInputType Type { get; }
		Guid Id { get; set; }
		string Name { get; set; }
		double Value { get; set; }
		event EventHandler Set;
	}
}