using System;
using Vixen.Data.Value;
using Vixen.Sys;
using Vixen.Sys.Dispatch;

namespace FadeOut {
	class IntentHandler : IntentSegmentDispatch {
		private CommandHandler _commandDispatch;
		private Reducer _reducer;

		public IntentHandler() {
			_commandDispatch = new CommandHandler();
			_reducer = new Reducer();
		}

		// StartTime to EndTime represent the time span within the filter's whole TimeSpan
		// that apply to the next operation.
		public TimeSpan TimeSpan;
		public TimeSpan StartTime;
		public TimeSpan EndTime;

		public override void Handle(IIntentSegment<ColorValue> obj) {
			obj.StartValue = _reducer.Reduce(obj.StartValue, _StartPercentIntoFilter());
			obj.EndValue = _reducer.Reduce(obj.EndValue, _EndPercentIntoFilter());
		}

		public override void Handle(IIntentSegment<LightingValue> obj) {
			obj.StartValue = _reducer.Reduce(obj.StartValue, _StartPercentIntoFilter());
			obj.EndValue = _reducer.Reduce(obj.EndValue, _EndPercentIntoFilter());
		}

		public override void Handle(IIntentSegment<CommandValue> obj) {
			_commandDispatch.ReductionPercentage = _StartPercentIntoFilter();
			obj.StartValue.Command.Dispatch(_commandDispatch);
			obj.StartValue = new CommandValue(_commandDispatch.Command);

			_commandDispatch.ReductionPercentage = _EndPercentIntoFilter();
			obj.EndValue.Command.Dispatch(_commandDispatch);
			obj.EndValue = new CommandValue(_commandDispatch.Command);
		}

		private double _StartPercentIntoFilter() {
			return StartTime.TotalMilliseconds / TimeSpan.TotalMilliseconds;
		}

		private double _EndPercentIntoFilter() {
			return EndTime.TotalMilliseconds / TimeSpan.TotalMilliseconds;
		}
	}
}
