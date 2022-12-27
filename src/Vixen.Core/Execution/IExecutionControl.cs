namespace Vixen.Execution
{
	public interface IExecutionControl
	{
		void Start();
		void Stop();
		void Pause();
		void Resume();
	}
}