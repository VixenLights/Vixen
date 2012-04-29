using System;
using System.Drawing;
using Vixen.Commands;
using Vixen.Sys;

namespace Vixen.Data.Combinator {
	//CombinatorValue may be null because it's now a command instead of a value.
	public class ColorCombinator : Combinator<ColorCombinator, Color> {
		override public void Handle(IEvaluator<Color> obj) {
			// Black + White = White
			// Blue + Yellow = Green ([0,255,0] + [255,0,255] = [0,0,255]?)
			// Bah, just going with highest-wins on each component for now.
			if(CombinatorValue == null) {
				CombinatorValue = new ColorValue(obj.EvaluatorValue);
			} else {
				CombinatorValue.CommandValue = CombinatorValue.CommandValue.Combine(obj.EvaluatorValue);
			}
		}
	}
}
