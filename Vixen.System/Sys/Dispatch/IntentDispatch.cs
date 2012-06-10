using System;

namespace Vixen.Sys.Dispatch {
	abstract public class IntentDispatch : IAnyIntentHandler {
		virtual public void Handle(IIntent<float> obj) { }

		virtual public void Handle(IIntent<DateTime> obj) { }

		virtual public void Handle(IIntent<System.Drawing.Color> obj) { }

		virtual public void Handle(IIntent<long> obj) { }

		virtual public void Handle(IIntent<double> obj) { }

		virtual public void Handle(IIntent<LightingValue> obj) { }
	}
}
