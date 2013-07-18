using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Commands;
using Vixen.Sys.Dispatch;

namespace Vixen.Data.Evaluator
{
	public class CustomEvaluator : Evaluator, IAnyCommandHandler
	{
		public void Handle(Commands._8BitCommand obj)
		{
			throw new NotImplementedException();
		}

		public void Handle(Commands._16BitCommand obj)
		{
			throw new NotImplementedException();
		}

		public void Handle(Commands._32BitCommand obj)
		{
			throw new NotImplementedException();
		}

		public void Handle(Commands._64BitCommand obj)
		{
			throw new NotImplementedException();
		}

		public void Handle(Commands.ColorCommand obj)
		{
			throw new NotImplementedException();
		}

		public void Handle(CustomCommand obj)
		{
			EvaluatorValue = new CustomCommand(obj.CommandValue);
		}

		 
	}
}
