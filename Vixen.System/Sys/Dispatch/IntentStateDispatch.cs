using Vixen.Data.Value;

namespace Vixen.Sys.Dispatch {
	abstract public class IntentStateDispatch : IAnyIntentStateHandler {
		virtual public void Handle(IIntentState<ColorValue> obj) { }

		virtual public void Handle(IIntentState<LightingValue> obj) { }

		virtual public void Handle(IIntentState<PositionValue> obj) { }

		virtual public void Handle(IIntentState<CommandValue> obj) { }
	}
}
