using Vixen.Commands;
using Vixen.Data.Value;
using Vixen.Sys;
using Vixen.Sys.Dispatch;

namespace Vixen.Data.Evaluator
{
	public class _8BitEvaluator : Evaluator, IAnyCommandHandler
	{
		// Handling intents as an evaluator.
		public override void Handle(IIntentState<ColorValue> obj)
		{
			byte byteLevel = ColorValue.GetGrayscaleLevel(obj.GetValue().Color);
			EvaluatorValue = new _8BitCommand(byteLevel);
		}

		public override void Handle(IIntentState<LightingValue> obj)
		{
			EvaluatorValue = new _8BitCommand(obj.GetValue().Intensity*byte.MaxValue);
		}

		public override void Handle(IIntentState<PositionValue> obj)
		{
			EvaluatorValue = new _8BitCommand((byte) (byte.MaxValue*obj.GetValue().Position));
		}

		public override void Handle(IIntentState<CommandValue> obj)
		{
			obj.GetValue().Command.Dispatch(this);
		}

		// Additionally dispatching command intents and handling the command they wrap.
		public void Handle(_8BitCommand obj)
		{
			EvaluatorValue = obj;
		}

		public void Handle(_16BitCommand obj)
		{
			EvaluatorValue = new _8BitCommand(obj.CommandValue);
		}

		public void Handle(_32BitCommand obj)
		{
			EvaluatorValue = new _8BitCommand(obj.CommandValue);
		}

		public void Handle(_64BitCommand obj)
		{
			EvaluatorValue = new _8BitCommand(obj.CommandValue);
		}

		public void Handle(ColorCommand obj)
		{
			EvaluatorValue = new _8BitCommand(ColorValue.GetGrayscaleLevel(obj.CommandValue));
		}


		public void Handle(CustomCommand obj)
		{
			throw new System.NotImplementedException();
		}
	}
}