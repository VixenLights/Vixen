using System;
using Vixen.Commands;

namespace Vixen.Data.Combinator.Color
{
	public class NaiveColorCombinator : Combinator<NaiveColorCombinator, System.Drawing.Color>
	{
		public override void Handle(ColorCommand obj)
		{
			if (CombinatorValue == null) {
				CombinatorValue = obj;
			}
			else {
				System.Drawing.Color value1 = CombinatorValue.CommandValue;
				System.Drawing.Color value2 = obj.CommandValue;
				CombinatorValue = _MergeColorNaively(value1, value2);
			}
		}

		private ICommand<System.Drawing.Color> _MergeColorNaively(System.Drawing.Color value1, System.Drawing.Color value2)
		{
			int a = Math.Max(value1.A, value2.A);
			int r = (value1.R + value2.R) >> 1;
			int g = (value1.G + value2.G) >> 1;
			int b = (value1.B + value2.B) >> 1;
			return new ColorCommand(System.Drawing.Color.FromArgb(a, r, g, b));
		}
	}
}