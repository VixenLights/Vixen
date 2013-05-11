using Common.StateMach;

namespace VixenModules.App.SimpleSchedule {
	static class States {
		static public IState<IScheduledItemStateObject> WaitingState;
		static public IState<IScheduledItemStateObject> PreExecuteState;
		static public IState<IScheduledItemStateObject> ExecutingState;
		static public IState<IScheduledItemStateObject> PostExecuteState;
		static public IState<IScheduledItemStateObject> CompletedState;
	}
}
