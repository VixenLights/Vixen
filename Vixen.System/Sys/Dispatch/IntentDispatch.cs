using Vixen.Data.Value;

namespace Vixen.Sys.Dispatch {
	abstract public class IntentDispatch : IAnyIntentHandler {
		virtual public void Handle(IIntent<LightingValue> obj) { }

		virtual public void Handle(IIntent<PositionValue> obj) { }

		virtual public void Handle(IIntent<CommandValue> obj) { }

		virtual public void Handle(IIntent<ColorValue> obj) { }
	}
}
