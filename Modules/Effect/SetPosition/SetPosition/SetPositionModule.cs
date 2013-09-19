using System;
using Common.ValueTypes;
using Vixen.Data.Value;
using Vixen.Intent;
using Vixen.Module;
using Vixen.Module.Effect;
using Vixen.Sys;
using Vixen.Sys.Attribute;

namespace VixenModules.Effect.SetPosition
{
	public class SetPositionModule : EffectModuleInstanceBase
	{
		private SetPositionData _data;
		private EffectIntents _effectIntents;

		public override IModuleDataModel ModuleData
		{
			get { return _data; }
			set { _data = (SetPositionData) value; }
		}

		[Value]
		public Percentage StartPosition
		{
			get { return _data.StartPosition; }
			set
			{
				_data.StartPosition = value;
				IsDirty = true;
			}
		}

		[Value]
		public Percentage EndPosition
		{
			get { return _data.EndPosition; }
			set
			{
				_data.EndPosition = value;
				IsDirty = true;
			}
		}

		protected override void _PreRender()
		{
			_effectIntents = new EffectIntents();

			foreach (ElementNode node in TargetNodes) {
				foreach (Element element in node.GetElementEnumerator()) {
					PositionValue startPosition = new PositionValue(StartPosition);
					PositionValue endPosition = new PositionValue(EndPosition);
					IIntent intent = new PositionIntent(startPosition, endPosition, TimeSpan);
					_effectIntents.AddIntentForElement(element.Id, intent, TimeSpan.Zero);
				}
			}
		}

		protected override EffectIntents _Render()
		{
			return _effectIntents;
		}
	}
}