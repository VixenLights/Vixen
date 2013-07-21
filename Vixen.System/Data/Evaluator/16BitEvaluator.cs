using Vixen.Commands;
using Vixen.Data.Value;
using Vixen.Sys;

namespace Vixen.Data.Evaluator
{
	//public class _16BitEvaluator : Evaluator<_16BitEvaluator, ushort> {
	public class _16BitEvaluator : Evaluator
	{
		public override void Handle(IIntentState<ColorValue> obj)
		{
			byte byteLevel = ColorValue.GetGrayscaleLevel(obj.GetValue().Color);
			EvaluatorValue = new _16BitCommand(byteLevel);
		}

		public override void Handle(IIntentState<LightingValue> obj)
		{
			EvaluatorValue = new _16BitCommand((ushort)(ushort.MaxValue * obj.GetValue().Intensity));
		}

		public override void Handle(IIntentState<PositionValue> obj)
		{
			EvaluatorValue = new _16BitCommand((ushort)(ushort.MaxValue * obj.GetValue().Position));
		}

		public override void Handle(IIntentState<CommandValue> obj)
		{
			obj.GetValue().Command.Dispatch(this);
		}

		public void Handle(_8BitCommand obj)
		{
			EvaluatorValue = new _16BitCommand(obj.CommandValue);
		}

		public void Handle(_16BitCommand obj)
		{
			EvaluatorValue = obj;
		}

		public void Handle(_32BitCommand obj)
		{
			EvaluatorValue = new _16BitCommand(obj.CommandValue);
		}

		public void Handle(_64BitCommand obj)
		{
			EvaluatorValue = new _16BitCommand(obj.CommandValue);
		}

		public void Handle(ColorCommand obj)
		{
			EvaluatorValue = new _16BitCommand(ColorValue.GetGrayscaleLevel(obj.CommandValue));
		}
	}
}