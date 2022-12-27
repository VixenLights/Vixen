namespace Vixen.Sys.State.Execution.Behavior
{
	internal class StandardClosedBehavior
	{
		public static void Run()
		{
			Vixen.Sys.Execution.SystemTime.Stop();
		}
	}
}