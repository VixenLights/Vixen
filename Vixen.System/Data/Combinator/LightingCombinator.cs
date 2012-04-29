using System;
using System.Drawing;
using Vixen.Commands;
using Vixen.Sys;

namespace Vixen.Data.Combinator {
	/// <summary>
	/// Intensity: Highest wins.
	/// Color: Highest wins of each component.
	/// </summary>
	public class LightingCombinator : Combinator<LightingCombinator, LightingValue> {
		//CombinatorValue may be null because it's now a command instead of a value.
		public override void Handle(IEvaluator<LightingValue> obj) {
			if(CombinatorValue == null) {
				Color color = obj.EvaluatorValue.Color;
				double intensity = obj.EvaluatorValue.Intensity;
				CombinatorValue = new LightingValueCommand(new LightingValue(color, intensity));
			} else {
				Color color = CombinatorValue.CommandValue.Color.Combine(obj.EvaluatorValue.Color);
				double intensity = Math.Max(CombinatorValue.CommandValue.Intensity, obj.EvaluatorValue.Intensity);
				CombinatorValue.CommandValue = new LightingValue(color, intensity);
			}
		}

		public override void Handle(IEvaluator<Color> obj) {
			if(CombinatorValue == null) {
				Color color = obj.EvaluatorValue;
				CombinatorValue = new LightingValueCommand(new LightingValue(color, 1));
			} else {
				Color color = CombinatorValue.CommandValue.Color.Combine(obj.EvaluatorValue);
				CombinatorValue.CommandValue = new LightingValue(color, CombinatorValue.CommandValue.Intensity);
			}
		}
	}
}
