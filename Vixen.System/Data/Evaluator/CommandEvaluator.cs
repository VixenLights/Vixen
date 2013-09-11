using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Commands;
using Vixen.Data.Value;
using Vixen.Sys;
namespace Vixen.Data.Evaluator
{
	public class CommandEvaluator: Evaluator
	{
		public override void Handle(IIntentState<Value.CommandValue> obj)
		{
			
			EvaluatorValue = new Vixen.Commands.UnknownValueCommand(obj.GetValue());
			
		}
		
	}
}
