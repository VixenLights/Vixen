using Common.StateMach;

namespace VixenModules.App.SimpleSchedule
{
	internal static class States
	{
		public static IState<IScheduledItemStateObject> WaitingState;
		public static IState<IScheduledItemStateObject> PreExecuteState;
		public static IState<IScheduledItemStateObject> ExecutingState;
		public static IState<IScheduledItemStateObject> PostExecuteState;
		public static IState<IScheduledItemStateObject> CompletedState;
	}
}