using Vixen.Data.Value;

namespace Vixen.Sys.Dispatch {
	abstract public class IntentSegmentDispatch : IAnyIntentSegmentHandler {
		virtual public void Handle(IIntentSegment<LightingValue> obj) { }

		virtual public void Handle(IIntentSegment<PositionValue> obj) { }

		virtual public void Handle(IIntentSegment<ColorValue> obj) { }

		virtual public void Handle(IIntentSegment<CommandValue> obj) { }
	}
}
