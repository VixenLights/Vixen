﻿using Vixen.Data.Value;
using Vixen.Sys;
using Vixen.Sys.Dispatch;

namespace VixenModules.Preview.VixenPreview.Shapes
{
	public class FullColorIntentHandler:IntentStateDispatch
	{
		private Color _color = Color.Empty;

		public Color GetFullColor(IIntentState state)
		{
			_color = Color.Empty;
			state.Dispatch(this);
			return _color;;
		}

		public override void Handle(IIntentState<RGBValue> obj)
		{
			_color = obj.GetValue().FullColorWithAlpha;
		}

		public override void Handle(IIntentState<LightingValue> obj)
		{
			_color = obj.GetValue().FullColorWithAlpha;
		}

		public override void Handle(IIntentState<DiscreteValue> obj)
		{
			_color = Color.Empty;
		}

	}
}
