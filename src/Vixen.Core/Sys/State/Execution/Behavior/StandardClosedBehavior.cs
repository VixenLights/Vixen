namespace Vixen.Sys.State.Execution.Behavior
{
	internal class StandardClosedBehavior
	{
		public static void Run()
		{
			Sys.Execution.SystemTime.Stop();
		}
	}
}