using Vixen.Data.Value;
using Vixen.Intent;
using Vixen.Sys;
using Vixen.Sys.Dispatch;

namespace VixenModules.OutputFilter.Color {
	abstract class ColorComponentFilter : IntentStateDispatch {
		private IIntentState _intentValue;

		// Not going to assume how the result is going to be used, so
		// we're not going to filter to a command and strip out
		// the intent.
		public IIntentState Filter(IIntentState intentValue) {
			intentValue.Dispatch(this);
			return _intentValue;
		}

		public override void Handle(IIntentState<ColorValue> obj) {
			_intentValue = new StaticIntentState<ColorValue>(obj, FilterColorValue(obj.GetValue()));
		}

		public override void Handle(IIntentState<LightingValue> obj) {
			_intentValue = new StaticIntentState<LightingValue>(obj, FilterLightingValue(obj.GetValue()));
		}

		abstract public string FilterName { get; }

		abstract protected ColorValue FilterColorValue(ColorValue colorValue);

		abstract protected LightingValue FilterLightingValue(LightingValue lightingValue);
	}
}
