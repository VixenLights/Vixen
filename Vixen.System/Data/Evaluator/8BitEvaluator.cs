using System;
using System.Collections.Generic;
using System.Windows.Media.Animation;
using Vixen.Commands;
using Vixen.Data.Value;
using Vixen.Sys;

namespace Vixen.Data.Evaluator
{
	public class _8BitEvaluator : Evaluator
	{
		private static readonly Dictionary<Byte, _8BitCommand> CommandLookup = new Dictionary<byte, _8BitCommand>();

		static _8BitEvaluator()
		{
			for (int i = 0; i <= Byte.MaxValue; i++)
			{
				CommandLookup.Add((byte)i, new _8BitCommand(i));
			}
		}
		// Handling intents as an evaluator.
		public override void Handle(IIntentState<RGBValue> obj)
		{
			var i = (byte) (byte.MaxValue * obj.GetValue().Intensity);
			EvaluatorValue = CommandLookup[i];
		}

		public override void Handle(IIntentState<LightingValue> obj)
		{
			var i = (byte)(byte.MaxValue * obj.GetValue().Intensity);
			EvaluatorValue = CommandLookup[i];
		}

		public override void Handle(IIntentState<PositionValue> obj)
		{
			var i = (byte)(byte.MaxValue * obj.GetValue().Position);
			EvaluatorValue = CommandLookup[i];
		}

		public override void Handle(IIntentState<DiscreteValue> obj)
		{
			var i = (byte)(byte.MaxValue * obj.GetValue().Intensity);
			EvaluatorValue = CommandLookup[i];
		}

		public override void Handle(IIntentState<IntensityValue> obj)
		{
			var i = (byte)(byte.MaxValue * obj.GetValue().Intensity);
			EvaluatorValue = CommandLookup[i];
		}

	}
}