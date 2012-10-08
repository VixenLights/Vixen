using System;
using Vixen.Execution;
using Vixen.Sys;
using VixenModules.App.SimpleSchedule.Service;

namespace VixenModules.App.SimpleSchedule.StateObject {
	class ScheduledSequence : IScheduledItemStateObject {
		private ISequence _sequence;

		public ScheduledSequence(IScheduledItem item) {
			Start = ScheduledItemService.CalculateConcreteStartDateTime(item);
			End = ScheduledItemService.CalculateConcreteEndDateTime(item);
			_sequence = Vixen.Services.SequenceService.Instance.Load(item.ItemFilePath);
		}

		public DateTime Start { get; private set; }

		public DateTime End { get; private set; }

		public bool ItemIsValid {
			get { return _sequence != null; }
		}

		public void RequestContext() {
			if(_sequence != null) {
				Context = VixenSystem.Contexts.CreateSequenceContext(new ContextFeatures(ContextCaching.ContextLevelCaching), _sequence);
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
