namespace Vixen.Module.Input
{
	public interface IInputInput
	{
		event EventHandler ValueChanged;
		string Name { get; }
		double Value { get; set; }
	}
}