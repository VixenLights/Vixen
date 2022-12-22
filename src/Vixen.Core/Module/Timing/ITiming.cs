using Vixen.Execution;

namespace Vixen.Module.Timing
{
	public interface ITiming : IExecutionControl
	{
		TimeSpan Position { get; set; }
		bool SupportsVariableSpeeds { get; }
		float Speed { get; set; }
	}
}