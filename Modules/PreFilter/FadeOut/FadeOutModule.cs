using System;
using Vixen.Module.PreFilter;
using Vixen.Sys;

namespace FadeOut {
	public class FadeOutModule : PreFilterModuleInstanceBase {
		private IntentHandler _intentHandler;

		public FadeOutModule() {
			_intentHandler = new IntentHandler();
		}

		public override void AffectIntent(IIntentSegment intentSegment, TimeSpan filterRelativeStartTime, TimeSpan filterRelativeEndTime) {
			_intentHandler.TimeSpan = TimeSpan;
			_intentHandler.StartTime = filterRelativeStartTime;
			_intentHandler.EndTime = filterRelativeEndTime;
			intentSegment.Dispatch(_intentHandler);
		}

	}
}
