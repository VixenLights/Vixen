using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Commands;
using Vixen.Module.PostFilter;
using Vixen.Sys;
using Vixen.Sys.Output;

namespace Vixen.Data.Policy {
	public abstract class OutputDataPolicy : IDataPolicy {
		private OutputController.Output _output;
		private ControllerDataPolicy _controllerDataPolicy;

		protected OutputDataPolicy(OutputController.Output output, ControllerDataPolicy controllerDataPolicy) {
			if(output == null) throw new ArgumentNullException("output");
			if(controllerDataPolicy == null) throw new ArgumentNullException("controllerDataPolicy");

			_output = output;
			_controllerDataPolicy = controllerDataPolicy;
		}

		public ICommand GenerateCommand(IEnumerable<IIntentState> intentStates) {
			// Stage 1: Evaluate into a single type of data (possibly transforming).
			IEnumerable<IEvaluator> evaluators = EvaluateIntentStates(intentStates);
			// Stage 2: Combine values of that type.
			ICombinator combinator = CombineEvaluations(evaluators);

			return combinator.CombinatorValue;
		}

		protected virtual IEnumerable<IEvaluator> EvaluateIntentStates(IEnumerable<IIntentState> intentStates) {
			return _controllerDataPolicy.EvaluateIntentStates(intentStates);
		}

		protected virtual ICombinator CombineEvaluations(IEnumerable<IEvaluator> evaluators) {
			return _controllerDataPolicy.CombineEvaluations(evaluators);
		}
	}
}
