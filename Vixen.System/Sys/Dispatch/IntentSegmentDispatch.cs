using System;

namespace Vixen.Sys.Dispatch {
	abstract public class IntentSegmentDispatch : IAnyIntentSegmentHandler {
		virtual public void Handle(IIntentSegment<float> obj) { }

		virtual public void Handle(IIntentSegment<DateTime> obj) { }

		virtual public void Handle(IIntentSegment<System.Drawing.Color> obj) { }

		virtual public void Handle(IIntentSegment<long> obj) { }

		virtual public void Handle(IIntentSegment<double> obj) { }

		virtual public void Handle(IIntentSegment<LightingValue> obj) { }
	}
}
