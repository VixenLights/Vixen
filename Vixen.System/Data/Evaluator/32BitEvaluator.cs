using Vixen.Commands;
using Vixen.Data.Value;
using Vixen.Sys;

namespace Vixen.Data.Evaluator
{
	//public class _32BitEvaluator : Evaluator<_32BitEvaluator, uint> {
	public class _32BitEvaluator : Evaluator
	{
		public override void Handle(IIntentState<ColorValue> obj)
		{
			byte byteLevel = ColorValue.GetGrayscaleLevel(obj.GetValue().Color);
			EvaluatorValue = new _32BitCommand(byteLevel);
		}

		public override void Handle(IIntentState<LightingValue> obj)
		{
			EvaluatorValue = new _32BitCommand((uint)(uint.MaxValue * obj.GetValue().Intensity));
		}

		public override void Handle(IIntentState<PositionValue> obj)
		{
			EvaluatorValue = new _32BitCommand((uint) (uint.MaxValue * obj.GetValue().Position));
		}

		public override void Handle(IIntentState<CommandValue> obj)
		{
			obj.GetValue().Command.Dispatch(this);
		}

		public void Handle(_8BitCommand obj)
		{
			EvaluatorValue = new _32BitCommand(obj.CommandValue);
		}

		public void Handle(_16BitCommand obj)
		{
			EvaluatorValue = new _32BitCommand(obj.CommandValue);
		}

		public void Handle(_32BitCommand obj)
		{
			EvaluatorValue = obj;
		}

		public void Handle(_64BitCommand obj)
		{
			EvaluatorValue = new _32BitCommand(obj.CommandValue);
		}

		public void Handle(ColorCommand obj)
		{
			EvaluatorValue = new _32BitCommand(ColorValue.GetGrayscaleLevel(obj.CommandValue));
		}
	}
}