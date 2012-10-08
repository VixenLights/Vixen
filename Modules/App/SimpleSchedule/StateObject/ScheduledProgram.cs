using System;
using Vixen.Execution;
using Vixen.Services;
using Vixen.Sys;
using VixenModules.App.SimpleSchedule.Service;

namespace VixenModules.App.SimpleSchedule.StateObject {
	class ScheduledProgram : IScheduledItemStateObject {
		private IProgram _program;

		public ScheduledProgram(IScheduledItem item) {
			Start = ScheduledItemService.CalculateConcreteStartDateTime(item);
			End = ScheduledItemService.CalculateConcreteEndDateTime(item);
			_program = ApplicationServices.LoadProgram(item.ItemFilePath);
		}

		public DateTime Start { get; private set; }

		public DateTime End { get; private set; }

		public bool ItemIsValid {
			get { return _program != null; }
		}

		public void RequestContext() {
			if(_program != null) {
				Context = VixenSystem.Contexts.CreateProgramContext(new ContextFeatures(ContextCaching.ContextLevelCaching), _program);
			}
		}

		public IContext Context { get; set; }

		public void ReleaseContext() {
			if(Context != null) {
				VixenSystem.Contexts.ReleaseContext(Context);
			}
		}
	}
}
