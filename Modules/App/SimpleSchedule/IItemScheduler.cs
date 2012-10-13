namespace VixenModules.App.SimpleSchedule {
	interface IItemScheduler {
		void AddSequence(IScheduledItem item);
		void AddProgram(IScheduledItem item);
		void UpdateSequence(IScheduledItem item);
		void UpdateProgram(IScheduledItem item);
		void RemoveSequence(IScheduledItem item);
		void RemoveProgram(IScheduledItem item);
	}
}
